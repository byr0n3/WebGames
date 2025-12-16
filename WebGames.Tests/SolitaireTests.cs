using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using WebGames.Core;
using WebGames.Core.Cards;
using WebGames.Core.Exceptions;
using WebGames.Core.Games;
using WebGames.Core.Players;
using Xunit;

namespace WebGames.Tests
{
	public sealed class SolitaireTests
	{
		private readonly TestsServiceProvider provider;

		public SolitaireTests(TestsServiceProvider provider)
		{
			this.provider = provider;
		}

		[Fact]
		public void CanCreateAndDestroyGames()
		{
			var gameManager = this.provider.GetRequiredService<GameManager>();

			var (player, game) = SolitaireTests.Create(gameManager, Solitaire.DefaultConfiguration);

			SolitaireTests.AssertNewGame(gameManager, player, game);

			// The default configuration for solitaire allows the game to automatically start.
			Assert.Equal(GameState.Playing, game.State);

			// Assert that initialization went well.
			Assert.Equal(Enum.GetValues<CardSuit>().Length - 1, game.Foundations.Length);
			Assert.Equal(Solitaire.TableauCount, game.Tableaus.Length);
			Assert.Empty(game.Foundations.SelectMany(static (foundation) => foundation));

			// Assert the tableau stacks got filled.
			for (var i = 0; i < game.Tableaus.Length; i++)
			{
				var tableau = game.Tableaus[i];

				Assert.Equal(i + 1, tableau.Count);
			}

			// Assert the talon got filled with the remaining cards in the deck.
			var tableauSum = game.Tableaus.Sum(static (tableau) => tableau.Count);
			Assert.Equal(CardStack.DeckSize - tableauSum, game.Talon.Count);

			gameManager.Leave(game, player);

			// Leaving the game should cause the game to have no players,
			// and the game manager to have no games (because the game was empty).
			Assert.Empty(game.CurrentPlayers);
			Assert.Empty(gameManager.Games);
		}

		[Fact]
		public void CanRespectCustomGameConfiguration()
		{
			var gameManager = this.provider.GetRequiredService<GameManager>();

			var configuration = Solitaire.DefaultConfiguration with
			{
				MinPlayers = 1,
				MaxPlayers = 2,
				AutoStart = false,
			};

			var (player, game) = SolitaireTests.Create(gameManager, configuration);

			SolitaireTests.AssertNewGame(gameManager, player, game);

			// Assert the configuration overrides.
			Assert.Equal(1, game.Configuration.MinPlayers);
			Assert.Equal(2, game.Configuration.MaxPlayers);
			// We expect `Idle` because we disabled `AutoStart`.
			Assert.Equal(GameState.Idle, game.State);

			gameManager.Leave(game, player);

			Assert.Empty(game.CurrentPlayers);
			Assert.Empty(gameManager.Games);
		}

		[Fact]
		public void CanValidatePlayerType()
		{
			var gameManager = this.provider.GetRequiredService<GameManager>();

			var invalidPlayer = new DummyPlayer();

			// Trying to create a game using the wrong player type should throw an exception.
			Assert.Throws<PlayerTypeException>(() => gameManager.Create<Solitaire>(Solitaire.DefaultConfiguration, invalidPlayer));

			// Create a standard game that can hold 2 players.
			var (_, game) = SolitaireTests.Create(gameManager, Solitaire.DefaultConfiguration with { MaxPlayers = 2, AutoStart = false });

			// Trying to join a game using the wrong player type should throw an exception.
			Assert.Throws<PlayerTypeException>(() => gameManager.TryGetOrJoin(game, invalidPlayer));

			// Trying to leave using the wrong player type should throw an exception.
			Assert.Throws<PlayerTypeException>(() => gameManager.Leave(game, invalidPlayer));
		}

		private static (SolitairePlayer, Solitaire) Create(GameManager gameManager, GameConfiguration configuration)
		{
			var player = new SolitairePlayer
			{
				Id = 1,
				DisplayName = "Test Player",
			};

			var game = gameManager.Create<Solitaire>(configuration, player);

			return (player, game);
		}

		[AssertionMethod]
		private static void AssertNewGame<TPlayer, TGame>(GameManager gameManager, TPlayer player, TGame? game)
			where TPlayer : IPlayer
			where TGame : IGame
		{
			Assert.NotNull(game);

			// A new game should only have 1 player: the host.
			Assert.Single(game.CurrentPlayers);
			Assert.Contains(player, game.CurrentPlayers);

			// The game manager should have the created game in its list.
			Assert.Single(gameManager.Games);
			Assert.Contains(game, gameManager.Games);
		}

		private sealed class DummyPlayer : IPlayer
		{
			public int Id { get; init; }

			public string DisplayName { get; init; } = null!;

			public bool Equals(IPlayer? other) =>
				(other is not null) && (this.Id == other.Id);

			public override int GetHashCode() =>
				this.Id;
		}
	}
}
