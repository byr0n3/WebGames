using System;
using System.Collections.Generic;
using System.Linq;
using WebGames.Core.Events;

namespace WebGames.Core
{
	/// <summary>
	/// Defines a contract for game implementations.
	/// </summary>
	public interface IGame : IDisposable
	{
		/// <summary>
		/// Delegate type used by the <see cref="IGame.GameUpdated"/> event to notify listeners of changes
		/// to a game instance.
		/// </summary>
		public delegate void OnGameUpdated(IGame game, GameUpdatedArgs args);

		/// <summary>
		/// Gets the unique identifier for the game.
		/// </summary>
		public string Code { get; }

		/// <summary>
		/// Gets a read-only list of players that have joined the game.
		/// </summary>
		public IReadOnlyList<IPlayer> CurrentPlayers { get; }

		/// <summary>
		/// Gets the game configuration that defines the game's parameters.
		/// </summary>
		public GameConfiguration Configuration { get; }

		/// <summary>
		/// Gets the current lifecycle phase of the game.
		/// </summary>
		public GameState State { get; }

		/// <summary>
		/// Indicates whether the game is currently able to accept additional players.
		/// </summary>
		public bool Joinable =>
			(this.CurrentPlayers.Count < this.Configuration.MaxPlayers);

		/// <summary>
		/// Fires whenever a change occurs within a game instance.
		/// </summary>
		public event OnGameUpdated? GameUpdated;

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
		/// <see langword="true"/> if the player is contained in the game's <see cref="CurrentPlayers"/> collection; otherwise, <see langword="false"/>.
		/// </returns>
		public bool ContainsPlayer(IPlayer player) =>
			this.CurrentPlayers.Contains(player);
	}
}
