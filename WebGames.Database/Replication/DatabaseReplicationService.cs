using System;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;
using Npgsql.Replication.PgOutput.Messages;
using WebGames.Database.Models.Options;

namespace WebGames.Database.Replication
{
	public sealed partial class DatabaseReplicationService : BackgroundService
	{
		private readonly DatabaseSynchronizationOptions options;
		private readonly IDbContextFactory<WebGamesDbContext> dbFactory;
		private readonly ILogger<DatabaseReplicationService> logger;

		private IRelationalTypeMappingSource typeMappings = null!;
		private IStructuralTypeMaterializerSource typeMaterializer = null!;
		private FrozenDictionary<string, IEntityType> entityTypes = null!;

		public DatabaseReplicationService(IOptions<DatabaseSynchronizationOptions> options,
										  ILogger<DatabaseReplicationService> logger,
										  IDbContextFactory<WebGamesDbContext> dbFactory)
		{
			this.logger = logger;
			this.dbFactory = dbFactory;
			this.options = options.Value;
		}

		/// <summary>
		/// Starts logical replication for the configured database.
		/// </summary>
		/// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the replication operation.</param>
		/// <returns>A <see cref="Task"/> that completes when the replication loop has stopped, typically due to cancellation.</returns>
		protected override async Task ExecuteAsync(CancellationToken token)
		{
			if (!this.options.IsValid)
			{
				this.logger.LogError("Database Synchronization options haven't been configured. No replication updates will be parsed.");
				return;
			}

			await this.InitializeAsync(token).ConfigureAwait(false);

			var connection = new LogicalReplicationConnection(this.options.ConnectionString);

			await using (connection.ConfigureAwait(false))
			{
				await connection.Open(token).ConfigureAwait(false);

				var slot = await connection.CreatePgOutputReplicationSlot(this.options.Slot,
																		  true,
																		  LogicalSlotSnapshotInitMode.NoExport,
																		  false,
																		  token)
										   .ConfigureAwait(false);

				var replicationOptions = new PgOutputReplicationOptions(DatabaseSynchronizationOptions.PublicationName,
																		PgOutputProtocolVersion.V1,
																		true);

				this.logger.LogDebug("Started listening for replication messages");

				await foreach (var message in connection.StartReplication(slot, replicationOptions, token).ConfigureAwait(false))
				{
					await this.HandleAsync(message, token).ConfigureAwait(false);

					connection.SetReplicationStatus(message.WalEnd);
				}
			}

			this.logger.LogDebug("Stopped listening for replication messages");
		}

		/// <summary>
		/// Loads internal type mapping services, materializer source, and entity type mappings
		/// required for database synchronization.
		/// </summary>
		/// <param name="token">A <see cref="CancellationToken"/> used to cancel the initialization operation.</param>
		/// <returns>A <see cref="Task"/> that completes when initialization is finished.</returns>
		/// <exception cref="Exception">
		/// Thrown when an entity type does not have an associated table name, indicating a model configuration issue.
		/// </exception>
		[SuppressMessage("Usage", "EF1001")]
		private async Task InitializeAsync(CancellationToken token)
		{
			var db = await this.dbFactory.CreateDbContextAsync(token).ConfigureAwait(false);

			await using (db.ConfigureAwait(false))
			{
				this.typeMappings = GetInternalServiceProvider(db).GetRequiredService<IRelationalTypeMappingSource>();

				this.typeMaterializer = db.GetDependencies().StateManager.EntityMaterializerSource;

				this.entityTypes = db.Model
									 .GetEntityTypes()
									 .Select(static (type) => new
									 {
										 Table = type.GetTableName(),
										 Type = type,
									 })
									 .Where(static (e) => e.Table is not null)
									 .ToFrozenDictionary(static (e) => e.Table!, static (e) => e.Type);
			}

			return;

			[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_InternalServiceProvider")]
			static extern System.IServiceProvider GetInternalServiceProvider(DbContext @this);
		}

		/// <summary>
		/// Handles a replication message by delegating to a more specific handler if the message is an update.
		/// </summary>
		/// <param name="message">The <see cref="PgOutputReplicationMessage"/> to process.</param>
		/// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous handling of the message.</returns>
		private Task HandleAsync(PgOutputReplicationMessage message, CancellationToken token) =>
			(message) switch
			{
				InsertMessage insert => this.HandleAsync(insert.Relation.RelationName, insert.NewRow, ReplicationType.Inserted, token),
				UpdateMessage update => this.HandleAsync(update.Relation.RelationName, update.NewRow, ReplicationType.Updated, token),
				_                    => Task.CompletedTask,
			};

		/// <summary>
		/// Handles a replication message for a specific table by retrieving the entity type, reading the row data,
		/// materializing the entity, and notifying subscribers about the change.
		/// </summary>
		/// <param name="table">The name of the database table to which the replication tuple belongs.</param>
		/// <param name="row">The replication tuple containing the column values for the row.</param>
		/// <param name="type">The type of replication operation (Inserted, Updated, Deleted, etc.) that has occurred.</param>
		/// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous handling of the replication message.</returns>
		private async Task HandleAsync(string table, ReplicationTuple row, ReplicationType type, CancellationToken token)
		{
			if (!this.entityTypes.TryGetValue(table, out var entityType))
			{
				this.logger.LogError("Unable to find entity type for table: {Table}", table);
				return;
			}

			this.logger.LogDebug("Incoming `{Type}` message for entity: {Entity}", type.ToString(), entityType.Name);

			// Read all the columns of the updated entity.
			var values = await this.ReadAsync(row, entityType, token).ConfigureAwait(false);

			var materializer = this.typeMaterializer.GetMaterializer(entityType);

			object entity;

			// @todo Don't initialize a new database context every time
			var db = await this.dbFactory.CreateDbContextAsync(token).ConfigureAwait(false);

			await using (db.ConfigureAwait(false))
			{
				// Let EF Core create a new instance of the entity.
				entity = materializer.Invoke(new MaterializationContext(new ValueBuffer(values), db));
			}

			DatabaseReplicationService.InvokeSubscribers(entity, entityType.ClrType, type);
		}
	}
}
