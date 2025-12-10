using System;

namespace WebGames.Core
{
	public interface IPlayer : IEquatable<IPlayer>
	{
		/// <summary>
		/// Gets the unique identifier for the player.
		/// </summary>
		public int Id { get; }

		/// <summary>
		/// Gets the display name for the player.
		/// </summary>
		public string DisplayName { get; }
	}
}
