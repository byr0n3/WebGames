using System;
using System.Collections.Generic;
using System.Linq;

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
		/// Gets a value indicating whether the game can currently accept additional players.
		/// </summary>
		/// <remarks>
		/// A game is considered joinable when one of the following conditions is true:
		/// <list type="bullet">
		/// <item>The game configuration permits joining after the game has started (<see cref="GameConfiguration.CanJoinInProgress"/> is <see langword="true"/>).</item>
		/// <item>The game is still in the <c>Idle</c> state.</item>
		/// </list>
		/// In addition, the current number of players must be less than the maximum allowed by the game's configuration (<see cref="GameConfiguration.MaxPlayers"/>).
		/// </remarks>
		/// <value>
		/// <see langword="true"/> if the game can accept more players; otherwise, <see langword="false"/>.
		/// </value>
		public bool Joinable =>
			(this.Players.Count < this.Configuration.MaxPlayers);

		/// <summary>
		/// Adds the specified player to the game.
		/// </summary>
		/// <param name="player">The player that is requesting to join the game.</param>
		internal void Join(IPlayer player);

		/// <summary>
		/// Removes the specified player from the game.
		/// </summary>
		/// <param name="player">The player that is leaving the game.</param>
		/// <remarks>If the player is not currently a member of the game, the call has no effect.</remarks>
		internal void Leave(IPlayer player);

		/// <summary>
		/// Checks if the specified <see cref="IPlayer"/> is present in the game's player roster.
		/// </summary>
		/// <param name="player">The player whose membership in the game should be verified.</param>
		/// <returns>
		/// <see langword="true"/> if the player is contained in the game's <see cref="IGame.Players"/> collection; otherwise, <see langword="false"/>.
		/// </returns>
		public bool ContainsPlayer(IPlayer player) =>
			this.Players.Contains(player);
	}
}
