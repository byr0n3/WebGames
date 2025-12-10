using System.Runtime.InteropServices;

namespace WebGames.Core
{
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
		public int MaxPLayers { get; init; }

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

		/// <summary>
		/// Can players join after the game has already started?
		/// </summary>
		/// <remarks>A player would most-likely join as a spectator.</remarks>
		public bool CanJoinInProgress { get; init; }
	}
}
