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
		Idle,

		/// <summary>
		/// The game has started and is in-progress.
		/// </summary>
		Playing,

		/// <summary>
		/// The game has finished.
		/// </summary>
		Done,
	}
}
