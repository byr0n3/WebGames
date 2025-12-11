using System.ComponentModel.DataAnnotations;
using WebGames.Core;

namespace WebGames.Models.Requests
{
	internal sealed class CreateGameModel
	{
		[Required] public GameType? Type { get; set; }

		public GameVisibility Visibility { get; set; }
	}

	internal enum GameType
	{
		Solitaire,
		Blackjack,
		Uno,
	}
}
