using System.Text.Json.Serialization;
using Elegance.Enums;

namespace WebGames.Core
{
	/// <summary>
	/// Represents the current lifecycle phase of a game.
	/// </summary>
	[Enum]
	[JsonConverter(typeof(JsonGameStateConverter))]
	public enum GameState
	{
		/// <summary>
		/// The game is waiting to start.
		/// </summary>
		[EnumValue("idle")]
		Idle = 0,

		/// <summary>
		/// The game has started and is in-progress.
		/// </summary>
		[EnumValue("playing")]
		Playing,
	}
}
