using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using WebGames.Core.Cards;

namespace WebGames.Web.Components
{
	public sealed partial class PlayingCardUi : ComponentBase
	{
		[Parameter] [EditorRequired] public PlayingCard Card { get; set; }

		[Parameter] public string Class { get; set; } = string.Empty;

		[Parameter] public bool? Hidden { get; set; }

		[Parameter] public EventCallback<PlayingCard> Clicked { get; set; }

		private bool Clickable =>
			this.Clicked.HasDelegate;

		public bool Flippable =>
			this.Hidden is not null;

		private Task ClickedAsync() =>
			this.Clicked.InvokeAsync(this.Card);
	}
}
