using System;
using System.Collections.Generic;

namespace WebGames.Database.Replication
{
	public sealed partial class DatabaseReplicationService
	{
		private static readonly Dictionary<Type, List<ReplicationSubscription>> subscriptions = new();

		/// <summary>
		/// Registers a callback to be invoked when a synchronization message for the specified entity type is received.
		/// </summary>
		/// <typeparam name="T">The entity type to subscribe to.</typeparam>
		/// <param name="callback">The action to execute when an entity of type <typeparamref name="T"/> is synchronized.</param>
		/// <returns>A <see cref="ReplicationSubscription"/> representing the subscription that can be used to unsubscribe.</returns>
		public static ReplicationSubscription Subscribe<T>(Action<T, ReplicationType> callback)
		{
			var type = typeof(T);
			var subscription = new ReplicationSubscription(type, callback);

			if (DatabaseReplicationService.subscriptions.TryGetValue(type, out var subscribers))
			{
				subscribers.Add(subscription);
			}
			else
			{
				DatabaseReplicationService.subscriptions.Add(type, [subscription]);
			}

			return subscription;
		}

		/// <summary>
		/// Unregisters a subscription to database synchronization events.
		/// </summary>
		/// <param name="subscription">The subscription to remove from the service's internal list.</param>
		public static void Unsubscribe(ReplicationSubscription subscription)
		{
			if (DatabaseReplicationService.subscriptions.TryGetValue(subscription.Type, out var subscribers))
			{
				subscribers.Remove(subscription);
			}
		}

		/// <summary>
		/// Invokes all registered <see cref="ReplicationSubscription"/> instances for the specified entity type.
		/// </summary>
		/// <param name="value">The entity instance that was synchronized.</param>
		/// <param name="entityType">The CLR type of the entity that was synchronized.</param>
		private static void InvokeSubscribers(object value, Type entityType, ReplicationType type)
		{
			if (!DatabaseReplicationService.subscriptions.TryGetValue(entityType, out var list))
			{
				return;
			}

			// Create a copy of the subscriptions to avoid a subscriber being added/removed as we're invoking updates,
			// for example, when a subscriber unsubscribes itself in the callback.
			foreach (var subscriber in list.ToArray())
			{
				subscriber.Invoke(value, entityType, type);
			}
		}
	}
}
