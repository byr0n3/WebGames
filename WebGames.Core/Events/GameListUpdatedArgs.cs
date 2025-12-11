namespace WebGames.Core.Events
{
	/// <summary>
	/// Event-data that represents changes to the current games-list.
	/// </summary>
	public readonly struct GameListUpdatedArgs
	{
		/// <summary>
		/// The game that was updated.
		/// </summary>
		public readonly IGame Game;

		/// <summary>
		/// The type of update that happened.
		/// </summary>
		public readonly GameListUpdatedType Type;

		internal GameListUpdatedArgs(IGame game, GameListUpdatedType type)
		{
			this.Game = game;
			this.Type = type;
		}
	}

	/// <summary>
	/// Represents the type of change that has occurred to a game.
	/// </summary>
	public enum GameListUpdatedType
	{
		/// <summary>
		/// A game was created.
		/// </summary>
		Created,

		/// <summary>
		/// A game has updated.
		/// </summary>
		/// <remarks>This event triggers when, for example, a player joins/leaves the game, or the game's state gets updated.</remarks>
		Updated,

		/// <summary>
		/// A game was removed.
		/// </summary>
		Removed,
	}
}
