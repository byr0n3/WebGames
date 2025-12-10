using System;
using System.Collections.Generic;

namespace WebGames.Core
{
	public interface IGame : IDisposable
	{
		/// <summary>
		/// Gets the unique identifier for the game.
		/// </summary>
		public string Code { get; }

		/// <summary>
		/// Gets a read-only list of players that have joined the game.
		/// </summary>
		public IReadOnlyList<IPlayer> Players { get; }

		/// <summary>
		/// Gets the game configuration that defines the game's parameters.
		/// </summary>
		public GameConfiguration Configuration { get; }

		/// <summary>
		/// Gets the current lifecycle phase of the game.
		/// </summary>
		public GameState State { get; }

		/// <summary>
		/// Attempts to add the specified player to the game.
		/// </summary>
		/// <param name="player">The player that is requesting to join the game.</param>
		/// <returns>
		/// <see langword="true"/> if the player was successfully added to the game; otherwise, <see langword="false"/> (e.g., when the game is full, not joinable, or the player is already part of the game).
		/// </returns>
		internal bool TryJoin(IPlayer player);

		/// <summary>
		/// Removes the specified player from the game.
		/// </summary>
		/// <param name="player">The player that is leaving the game.</param>
		/// <remarks>If the player is not currently a member of the game, the call has no effect.</remarks>
		internal void Leave(IPlayer player);
	}
}
