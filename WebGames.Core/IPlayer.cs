using System;

namespace WebGames.Core
{
	/// <summary>
	/// Defines a contract for player implementations that can join and play games.
	/// </summary>
	public interface IPlayer : IEquatable<IPlayer>
	{
		/// <summary>
		/// Gets the unique identifier for the player.
		/// </summary>
		public int Id { get; init; }

		/// <summary>
		/// Gets the display name for the player.
		/// </summary>
		public string DisplayName { get; init; }
	}
}
