using System;
using WebGames.Core.Players;

namespace WebGames.Core.Games
{
	/// <summary>
	/// Represents a single‑player solitaire game that can be instantiated.
	/// </summary>
	public sealed class Solitaire : BaseGame<SolitairePlayer>, ICreatableGame
	{
		/// <summary>
		/// Defines the default configuration used when creating a new <see cref="Solitaire"/> instance.
		/// </summary>
		public static readonly GameConfiguration DefaultConfiguration = new()
		{
			MinPlayers = 1,
			MaxPlayers = 1,
			AutoStart = true,
		};

		private Solitaire(string code, GameConfiguration configuration) : base(code, configuration)
		{
		}

		/// <inheritdoc/>
		public static IGame Create(string code, GameConfiguration configuration, IServiceProvider _) =>
			new Solitaire(code, configuration);
	}
}
