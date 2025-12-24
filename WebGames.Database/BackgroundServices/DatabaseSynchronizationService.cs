using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;
using Npgsql.Replication.PgOutput.Messages;
using WebGames.Database.Models.Options;

namespace WebGames.Database.BackgroundServices
{
	public sealed class DatabaseSynchronizationService : BackgroundService
	{
		public delegate void OnUpdateMessageReceived(string table, DatabaseUpdateData data);

		public static event OnUpdateMessageReceived? UpdateMessageReceived;

		private readonly DatabaseSynchronizationOptions options;
		private readonly ILogger<DatabaseSynchronizationService> logger;

		public DatabaseSynchronizationService(IOptions<DatabaseSynchronizationOptions> options,
											  ILogger<DatabaseSynchronizationService> logger)
		{
			this.logger = logger;
			this.options = options.Value;
		}

		protected override async Task ExecuteAsync(CancellationToken token)
		{
			if (!this.options.IsValid)
			{
				throw new Exception("Database Synchronization hasn't been configured properly.");
			}

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

				var replicationOptions =
					new PgOutputReplicationOptions(DatabaseSynchronizationOptions.PublicationName, PgOutputProtocolVersion.V1, true);

				this.logger.LogDebug("Started listening for replication messages");

				await foreach (var message in connection.StartReplication(slot, replicationOptions, token).ConfigureAwait(false))
				{
					await DatabaseSynchronizationService.HandleAsync(message, token).ConfigureAwait(false);

					connection.SetReplicationStatus(message.WalEnd);
				}
			}

			this.logger.LogDebug("Stopped listening for replication messages");
		}

		private static Task HandleAsync(PgOutputReplicationMessage message, CancellationToken token)
		{
			if (message is UpdateMessage update)
			{
				return DatabaseSynchronizationService.HandleAsync(update, token);
			}

			return Task.CompletedTask;
		}

		private static async Task HandleAsync(UpdateMessage message, CancellationToken token)
		{
			var table = message.Relation.RelationName;

			var dictionary = new Dictionary<string, object?>(message.NewRow.NumColumns, StringComparer.Ordinal);

			await foreach (var replicationValue in message.NewRow.WithCancellation(token).ConfigureAwait(false))
			{
				if (replicationValue.IsUnchangedToastedValue)
				{
					continue;
				}

				var column = replicationValue.GetFieldName();
				var value = !replicationValue.IsDBNull ? await replicationValue.Get(token).ConfigureAwait(false) : null;

				dictionary.Add(column, value);
			}

			// @todo Refactor
			// - reconstruct entity?
			// - event per entity type?
			// - different event-subscription system? (subscribe to specific IDs?)
			DatabaseSynchronizationService.UpdateMessageReceived?.Invoke(table, new DatabaseUpdateData(dictionary));
		}
	}

	public sealed class DatabaseUpdateData
	{
		private readonly FrozenDictionary<string, object?> data;

		public DatabaseUpdateData(Dictionary<string, object?> data) =>
			this.data = data.ToFrozenDictionary();

		public T? Get<T>(string column) =>
			(T?)this.data[column];

		public bool TryGet<T>(string column, [NotNullWhen(true)] out T? result)
		{
			if (!this.data.TryGetValue(column, out var value) || (value is null))
			{
				result = default;
				return false;
			}

			result = (T)value;
			return true;
		}
	}
}
