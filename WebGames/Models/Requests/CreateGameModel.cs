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

		[Range(1, 99)]
		[Display(Name = nameof(CreateGameModel.MinPlayers), ResourceType = typeof(CreateGameModelLocalization))]
		public int MinPlayers { get; set; }

		[Range(1, 99)]
		[Display(Name = nameof(CreateGameModel.MaxPlayers), ResourceType = typeof(CreateGameModelLocalization))]
		public int MaxPlayers { get; set; }

		[Display(Name = nameof(CreateGameModel.AutoStart), ResourceType = typeof(CreateGameModelLocalization))]
		public bool AutoStart { get; set; } = true;

		[Display(Name = nameof(CreateGameModel.AllowSpectators), ResourceType = typeof(CreateGameModelLocalization))]
		public bool AllowSpectators { get; set; } = true;
	}

	internal enum GameType
	{
		[Display(Name = nameof(GameType.Solitaire), ResourceType = typeof(GameLocalization))]
		Solitaire,

		[Display(Name = nameof(GameType.Blackjack), ResourceType = typeof(GameLocalization))]
		Blackjack,
	}
}
