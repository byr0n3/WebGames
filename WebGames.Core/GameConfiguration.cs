using System.Runtime.InteropServices;

namespace WebGames.Core
{
	/// <summary>
	/// Represents the immutable configuration settings for a game instance.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct GameConfiguration
	{
		/// <summary>
		/// The minimum amount of players that's required to start the game.
		/// </summary>
		public int MinPlayers { get; init; }

		/// <summary>
		/// The maximum amount of players that can join the game.
		/// </summary>
		public int MaxPlayers { get; init; }

		/// <summary>
		/// The visibility setting for the game, indicating whether it is publicly discoverable,
		/// limited to friends only, or private.
		/// </summary>
		public GameVisibility Visibility { get; init; }

		/// <summary>
		/// Should the game automatically start when it's able to?
		/// </summary>
		/// <remarks>
		/// A game can automatically start under one of the following conditions:
		/// <list type="bullet">
		/// <item>the minimum amount of players has joined and the start timer has expired.</item>
		/// <item>the maximum amount of players has joined.</item>
		/// </list>
		/// </remarks>
		public bool AutoStart { get; init; }
	}
}
