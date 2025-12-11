using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WebGames.Core.Events;
using WebGames.Core.Utilities;

namespace WebGames.Core
{
	/// <summary>
	/// Manages the lifecycle of games within the application.
	/// </summary>
	public sealed class GameManager : IDisposable
	{
		/// <summary>
		/// Represents the method signature for handlers that are invoked when the list of public games changes.
		/// </summary>
		public delegate void OnGameListUpdated(GameManager manager, GameListUpdatedArgs args);

		private readonly List<IGame> games;
		private readonly IServiceProvider services;

		/// <summary>
		/// Notifies subscribers that the list of games managed by <see cref="GameManager"/> has changed.
		/// </summary>
		public event OnGameListUpdated? GameListUpdated;

		/// <summary>
		/// Retrieves all games managed by this <see cref="GameManager"/> that are marked as <see cref="GameVisibility.Public"/>.
		/// </summary>
		public IEnumerable<IGame> Games =>
			// @todo Handle `GameVisibility.FriendsOnly`
			this.games.Where(static (g) => g.Configuration.Visibility == GameVisibility.Public);

		/// <inheritdoc cref="GameManager" />
		public GameManager(IServiceProvider services)
		{
			this.games = [];
			this.services = services;
		}

		/// <summary>
		/// Creates a new game of the specified type, registers it with the manager, and attempts to add the provided player to it.
		/// </summary>
		/// <typeparam name="TGame">The concrete game type that implements <see cref="IGame"/> and <see cref="ICreatableGame"/>.</typeparam>
		/// <param name="configuration">The configuration settings that define the game's parameters.</param>
		/// <param name="player">The player to be joined to the newly created game.</param>
		/// <returns>The newly created game instance implementing <see cref="IGame"/>.</returns>
		public TGame Create<TGame>(GameConfiguration configuration, IPlayer player)
			where TGame : IGame, ICreatableGame
		{
			var code = CodeGenerator.Generate();
			var game = TGame.Create(code, configuration, this.services);

			game.Join(player);

			this.games.Add(game);

			this.GameListUpdated?.Invoke(this, new GameListUpdatedArgs(game, GameListUpdatedType.Created));

			return (TGame)game;
		}

		/// <summary>
		/// Attempts to locate a game by its unique code and ensure the specified player can join it.
		/// If the game is found, is joinable, and the player is either already a participant or can be added,
		/// the method will optionally add the player to the game.
		/// </summary>
		/// <param name="code">The unique identifier of the game to retrieve.</param>
		/// <param name="player">The player requesting access to the game.</param>
		/// <param name="game">
		/// When the method returns <see langword="true"/>, contains the matching <see cref="IGame"/> instance;
		/// otherwise, set to <see langword="null"/>.
		/// </param>
		/// <returns>
		/// <see langword="true"/> if a matching game was found, is joinable, and the player is (or has become) a participant;
		/// otherwise, <see langword="false"/>.
		/// </returns>
		public bool TryGetOrJoin(string code, IPlayer player, [NotNullWhen(true)] out IGame? game)
		{
			game = this.FindGame(code);

			return game is not null && this.TryGetOrJoin(game, player);
		}

		/// <summary>
		/// Attempts to locate a game by its unique code and ensure the specified player can join it.
		/// If the game is found, is joinable, and the player is either already a participant or can be added,
		/// the method will optionally add the player to the game.
		/// </summary>
		/// <param name="game">The game that the player is attempting to join.</param>
		/// <param name="player">The player requesting access to the game.</param>
		/// <returns>
		/// <see langword="true"/> if a matching game was found, is joinable, and the player is (or has become) a participant;
		/// otherwise, <see langword="false"/>.
		/// </returns>
		public bool TryGetOrJoin(IGame game, IPlayer player)
		{
			var joinable = game.Joinable;
			var joined = game.ContainsPlayer(player);

			if (!joinable && !joined)
			{
				return false;
			}

			if (!joined)
			{
				game.Join(player);

				this.GameListUpdated?.Invoke(this, new GameListUpdatedArgs(game, GameListUpdatedType.Updated));
			}

			return true;
		}

		/// <summary>
		/// Removes the specified player from the given game. If the game becomes empty after the player leaves,
		/// the game is disposed and removed from the manager's collection.
		/// </summary>
		/// <param name="game">The game instance from which the player will be removed.</param>
		/// <param name="player">The player that is leaving the game.</param>
		public void Leave(IGame game, IPlayer player)
		{
			game.Leave(player);

			// If the game is empty, we should remove it from the games-list.
			if (game.CurrentPlayers.Count == 0)
			{
				game.Dispose();

				this.games.Remove(game);

				this.GameListUpdated?.Invoke(this, new GameListUpdatedArgs(game, GameListUpdatedType.Removed));
			}
			else
			{
				this.GameListUpdated?.Invoke(this, new GameListUpdatedArgs(game, GameListUpdatedType.Updated));
			}
		}

		private IGame? FindGame(string code) =>
			this.games.Find((g) => string.Equals(g.Code, code, StringComparison.Ordinal));

		/// <inheritdoc />
		public void Dispose()
		{
			foreach (var game in this.games)
			{
				game.Dispose();
			}

			this.games.Clear();
		}
	}
}
