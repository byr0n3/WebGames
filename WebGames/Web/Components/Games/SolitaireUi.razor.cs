using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WebGames.Core.Games;
using WebGames.Core.Players;

namespace WebGames.Web.Components.Games
{
	public sealed partial class SolitaireUi : ComponentBase, IAsyncDisposable
	{
		[Inject] public required IJSRuntime Js { get; init; }

		[Parameter] [EditorRequired] public Solitaire Game { get; set; }

		[Parameter] [EditorRequired] public SolitairePlayer Player { get; set; }

		private IJSObjectReference classRef = null!;

		protected override Task OnAfterRenderAsync(bool firstRender) =>
			firstRender ? this.InitializeAsync() : Task.CompletedTask;

		private async Task InitializeAsync()
		{
			var @ref = DotNetObjectReference.Create(this);

			this.classRef = await this.Js.InvokeConstructorAsync(nameof(Solitaire), @ref);
		}

		private void NextTalonCard() =>
			this.Game.NextTalonCard();

		private void HandleTalonCardClicked() =>
			this.Game.MoveTalonCardToFoundation();

		private void HandleTableauCardClicked(int tableauIndex) =>
			this.Game.MoveTableauCardToFoundation(tableauIndex);

		public ValueTask DisposeAsync() =>
			this.classRef.DisposeAsync();
	}
}
