using System.Collections.Generic;
using System.Diagnostics;
using WebGames.Core.Events;
using WebGames.Core.Exceptions;

namespace WebGames.Core.Games
{
	/// <summary>
	/// Provides a foundation for games that support player participation and basic lifecycle management.
	/// </summary>
	/// <typeparam name="TPlayer">
	/// The type of player that can join the game. <typeparamref name="TPlayer"/> must implement <see cref="IPlayer"/>.
	/// </typeparam>
	public abstract class BaseGame<TPlayer> : IGame
		where TPlayer : IPlayer
	{
		/// <inheritdoc/>
		public string Code { get; init; }

		/// <inheritdoc/>
		public GameConfiguration Configuration { get; init; }

		/// <inheritdoc/>
		public GameState State { get; private set; }

		/// <inheritdoc/>
		public event IGame.OnGameUpdated? GameUpdated;

		/// <summary>
		/// Holds the list of players participating in the game.
		/// </summary>
		protected readonly List<TPlayer> Players;

		/// <inheritdoc/>
		public IReadOnlyList<IPlayer> CurrentPlayers =>
			(IReadOnlyList<IPlayer>)this.Players.AsReadOnly();

		/// <inheritdoc cref="BaseGame{TPlayer}"/>
		protected BaseGame(string code, GameConfiguration configuration)
		{
			this.Code = code;
			this.Configuration = configuration;

			this.State = GameState.Idle;

			this.Players = new List<TPlayer>(configuration.MaxPlayers);
		}

		/// <inheritdoc/>
		public void Join(IPlayer player)
		{
			if (player is not TPlayer gamePlayer)
			{
				throw new PlayerTypeException(this.GetType(), typeof(TPlayer), player.GetType());
			}

			this.Players.Add(gamePlayer);

			this.GameUpdated?.Invoke(this, new GameUpdatedArgs(GameUpdatedType.PlayerJoined));

			// @todo Start timer, start shorter timer when max. players have been reached.
			if (this.Configuration.AutoStart && (this.Players.Count >= this.Configuration.MinPlayers))
			{
				this.Start();
			}
		}

		/// <inheritdoc/>
		public void Leave(IPlayer player)
		{
			if (player is not TPlayer gamePlayer)
			{
				throw new PlayerTypeException(this.GetType(), typeof(TPlayer), player.GetType());
			}

			this.Players.Remove(gamePlayer);

			this.GameUpdated?.Invoke(this, new GameUpdatedArgs(GameUpdatedType.PlayerLeft));

			// @todo Stop game?
		}

		/// <summary>
		/// Transitions the game to <see cref="GameState.Playing"/> and informs subscribers of the change.
		/// </summary>
		protected void Start()
		{
			this.State = GameState.Playing;

			this.GameUpdated?.Invoke(this, new GameUpdatedArgs(GameUpdatedType.StateUpdated));
		}

		/// <summary>
		/// Transitions the game to <see cref="GameState.Idle"/> and informs subscribers of the change.
		/// </summary>
		protected void Stop()
		{
			this.State = GameState.Idle;

			this.GameUpdated?.Invoke(this, new GameUpdatedArgs(GameUpdatedType.StateUpdated));
		}

		/// <inheritdoc/>
		public void Dispose() =>
			this.Players.Clear();
	}
}
