using System.Text.Json.Serialization;
using Elegance.Enums;

namespace WebGames.Core
{
	/// <summary>
	/// Defines the visibility level of a game.
	/// </summary>
	/// <remarks>This setting controls how the game appears in listings and who is allowed to join it.</remarks>
	[Enum]
	[JsonConverter(typeof(JsonGameVisibilityConverter))]
	public enum GameVisibility
	{
		/// <summary>
		/// The game is shown in the public games-list, so everyone can join it.
		/// </summary>
		[EnumValue("public")]
		Public,

		/// <summary>
		/// Friends of the host can see the game in the games-list, other users can't.
		/// </summary>
		/// <remarks>Anyone can still join the game using the game's code.</remarks>
		[EnumValue("friends-only")]
		FriendsOnly,

		/// <summary>
		/// The game is not visible to anyone.
		/// Players can only join using the game's code.
		/// </summary>
		[EnumValue("private")]
		Private,
	}
}
