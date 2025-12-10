using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using WebGames.Core.Extensions;
using WebGames.Core.Utilities;

namespace WebGames.Core
{
	public sealed class GameManager : IDisposable
	{
		private readonly List<IGame> games;
		private readonly IServiceProvider services;

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
		public IGame Create<TGame>(GameConfiguration configuration, IPlayer player)
			where TGame : IGame, ICreatableGame
		{
			var code = CodeGenerator.Generate();
			var game = TGame.Create(code, configuration, this.services);

			var joined = game.TryJoin(player);

			Debug.Assert(joined, "Unable to create newly created game");

			this.games.Add(game);

			return game;
		}

		/// <summary>
		/// Attempts to locate a game using the supplied join <paramref name="code"/> and, if the game is joinable and does not already contain the specified <paramref name="player"/>, adds the player to the game.
		/// </summary>
		/// <param name="code">The unique identifier for the game to join.</param>
		/// <param name="player">The player that is trying to join the game.</param>
		/// <param name="game">
		/// When the method returns <see langword="true"/>, receives the <see cref="IGame"/> instance that the player successfully joined; otherwise, receives <see langword="null"/>.
		/// </param>
		/// <returns><see langword="true"/> if the player was successfully added to the game; otherwise, <see langword="false"/>.</returns>
		public bool TryJoin(string code, IPlayer player, [NotNullWhen(true)] out IGame? game)
		{
			game = this.FindGame(code);

			if ((game?.Joinable != true) || (game.ContainsPlayer(player)))
			{
				game = null;
				return false;
			}

			var joined = game.TryJoin(player);

			if (!joined)
			{
				game = null;
			}

			return joined;
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

			if (game.Players.Count != 0)
			{
				return;
			}

			game.Dispose();

			this.games.Remove(game);
		}

		private IGame? FindGame(string code) =>
			this.games.Find((g) => string.Equals(g.Code, code, StringComparison.Ordinal));

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
