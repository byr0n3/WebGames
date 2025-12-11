using System;
using System.Collections.Generic;
using WebGames.Core.Players;

namespace WebGames.Core.Games
{
	public sealed class Solitaire : IGame, ICreatableGame
	{
		public static readonly GameConfiguration DefaultConfiguration = new()
		{
			MinPlayers = 1,
			MaxPlayers = 1,
			AutoStart = true,
		};

		public string Code { get; init; }
		public GameConfiguration Configuration { get; init; }

		public GameState State { get; private set; }

		private readonly List<SolitairePlayer> players;

		private Solitaire(string code, GameConfiguration configuration)
		{
			this.Code = code;
			this.Configuration = configuration;

			this.State = GameState.Idle;

			this.players = new List<SolitairePlayer>(configuration.MaxPlayers);
		}

		public IReadOnlyList<IPlayer> Players =>
			this.players;

		public void Join(IPlayer player) =>
			this.players.Add((SolitairePlayer)player);

		public void Leave(IPlayer player) =>
			this.players.RemoveAll((pl) => pl.Id == player.Id);

		public void Dispose() =>
			this.players.Clear();

		public static IGame Create(string code, GameConfiguration configuration, IServiceProvider _) =>
			new Solitaire(code, configuration);
	}
}
