using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Npgsql.Replication.PgOutput;
using WebGames.Database.Extensions;

namespace WebGames.Database.Replication
{
	public sealed partial class DatabaseReplicationService
	{
		/// <summary>
		/// Converts a <see cref="ReplicationTuple"/> representing a database row into an array of CLR values that match the
		/// properties of the supplied <see cref="IEntityType"/>.
		/// </summary>
		/// <param name="row">The replication tuple that contains the raw column values from the PostgreSQL logical replication stream.</param>
		/// <param name="entityType">The Entity Framework Core entity type that defines the mapping of columns to CLR properties.</param>
		/// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
		/// <returns>
		/// A <see cref="ValueTask{object?[]}"/> that completes with an array of objects whose indices correspond to the
		/// property positions defined by <paramref name="entityType"/>. The array may contain <see langword="null"/> entries for columns
		/// whose values are SQL <c>NULL</c> in the replication stream.
		/// </returns>
		/// <exception cref="Exception">
		/// Thrown if the <paramref name="row"/> contains a column name that does not map to any property of
		/// <paramref name="entityType"/>, indicating a schema mismatch or a missing property definition.
		/// </exception>
		private async ValueTask<object?[]> ReadAsync(ReplicationTuple row, IEntityType entityType, CancellationToken token)
		{
			var result = new object?[row.NumColumns];

			await foreach (var replicationValue in row.WithCancellation(token).ConfigureAwait(false))
			{
				if (replicationValue.IsUnchangedToastedValue)
				{
					continue;
				}

				// Get property from the entity's model (`GetFieldName` returns the column name).
				var column = replicationValue.GetFieldName();
				var property = entityType.GetPropertyByColumn(column);
				var index = property.GetIndex();

				if (index == -1)
				{
					this.logger.LogWarning("Couldn't find property index for property: {Property}", property.Name);
					continue;
				}

				object? value;

				// The value in the database is `null`, we don't have to read anything.
				if (replicationValue.IsDBNull)
				{
					value = null;
				}
				// If there's a registered mapping for the property, we need to use it when reading its value.
				else if (this.typeMappings?.FindMapping(property) is { } mapping)
				{
					var type = mapping.Converter?.ProviderClrType ?? mapping.ClrType;

					value = await DatabaseReplicationService.GetValueAsync(type, replicationValue, token).ConfigureAwait(false);

					// I don't THINK we have to use `mapping.Converter` here, as I'm pretty sure the materializer takes care of that.
				}
				// Read the database value normally.
				else
				{
					value = await replicationValue.Get(token).ConfigureAwait(false);
				}

				result[index] = value;
			}

			return result;
		}

		/// <summary>
		/// Reads a value from a replication tuple and converts it to the specified CLR type.
		/// </summary>
		/// <param name="type">The target .NET type to which the replication value should be converted.</param>
		/// <param name="value">The tuple value containg the raw database data.</param>
		/// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the read operation.</param>
		/// <returns>A <see cref="ValueTask{Object}"/> that completes with the converted value, or null if there isn't any.</returns>
		/// <exception cref="Exception">
		/// Thrown if the internal <see cref="GetAsync"/> method cannot be found via reflection.
		/// This typically indicates a mismatch between the requested type and the replication value.
		/// </exception>
		private static ValueTask<object?> GetValueAsync(Type type, ReplicationValue value, CancellationToken token)
		{
			const BindingFlags bindings = BindingFlags.Static | BindingFlags.NonPublic;

			// @todo Cache
			var method = typeof(DatabaseReplicationService).GetMethod(nameof(DatabaseReplicationService.GetAsync), bindings) ??
						 throw new Exception();

			return (ValueTask<object?>)method.MakeGenericMethod(type).Invoke(null, [value, token]);
		}

		// Wrapper function for `Get` as there's 2 get functions with almost the same signature,
		// causing `Type.GetMethod` to throw an exception.
		private static async ValueTask<object?> GetAsync<T>(ReplicationValue value, CancellationToken token) =>
			await value.Get<T>(token).ConfigureAwait(false);
	}
}
