using System.ComponentModel.DataAnnotations;
using WebGames.Core;
using WebGames.Resources;

namespace WebGames.Models.Requests
{
	internal sealed class CreateGameModel
	{
		[Required]
		[Display(Name = nameof(CreateGameModel.Type), ResourceType = typeof(CreateGameModelLocalization))]
		public GameType? Type { get; set; }

		[Display(Name = nameof(CreateGameModel.Visibility), ResourceType = typeof(CreateGameModelLocalization))]
		public GameVisibility Visibility { get; set; }
	}

	internal enum GameType
	{
		[Display(Name = nameof(GameType.Solitaire), ResourceType = typeof(GameLocalization))]
		Solitaire,

		[Display(Name = nameof(GameType.Blackjack), ResourceType = typeof(GameLocalization))]
		Blackjack,
	}
}
