using System;

namespace WebGames.Database.Replication
{
	/// <summary>
	/// Represents a subscription to database replication events for a specific entity type.
	/// </summary>
	/// <remarks>
	/// A <see cref="ReplicationSubscription"/> holds a reference to an entity type and a callback
	/// that should be executed when a synchronization message for that type is received.
	/// </remarks>
	public sealed class ReplicationSubscription
	{
		internal readonly Type Type;
		private readonly object callback;

		// @todo Should we call `GC.KeepAlive` on callback?
		internal ReplicationSubscription(Type type, object callback)
		{
			this.Type = type;
			this.callback = callback;
		}

		internal void Invoke(object value, Type entityType, ReplicationType type)
		{
			var actionMethod = typeof(Action<,>).MakeGenericType(entityType, typeof(ReplicationType));
			var method = actionMethod.GetMethod(nameof(Action.Invoke));

			method?.Invoke(this.callback, [value, type]);
		}
	}
}
