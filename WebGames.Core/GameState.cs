namespace WebGames.Core
{
	/// <summary>
	/// Represents the current lifecycle phase of a game.
	/// </summary>
	public enum GameState
	{
		/// <summary>
		/// The game is waiting to start.
		/// </summary>
		Idle = 0,

		/// <summary>
		/// The game has started.
		/// </summary>
		Started,
	}
}
