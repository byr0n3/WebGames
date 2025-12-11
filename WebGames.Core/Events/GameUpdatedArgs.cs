namespace WebGames.Core.Events
{
	/// <summary>
	/// Event-data that represents changes to a game.
	/// </summary>
	public readonly struct GameUpdatedArgs
	{
		/// <summary>
		/// The type of update that happened.
		/// </summary>
		public readonly GameUpdatedType Type;

		internal GameUpdatedArgs(GameUpdatedType type)
		{
			this.Type = type;
		}
	}

	/// <summary>
	/// Represents the type of change that has occurred to a game.
	/// </summary>
	public enum GameUpdatedType
	{
		/// <summary>
		/// The state of the game has updated.
		/// </summary>
		StateUpdated,

		/// <summary>
		/// A player joined the game.
		/// </summary>
		PlayerJoined,

		/// <summary>
		/// A player left the game.
		/// </summary>
		PlayerLeft,
	}
}
