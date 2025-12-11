using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using WebGames.Core;
using WebGames.Core.Events;
using WebGames.Core.Games;
using WebGames.Core.Players;
using WebGames.Extensions;
using WebGames.Services;

namespace WebGames.Web.Pages.Games
{
	public sealed partial class Index : ComponentBase, IDisposable
	{
		[Inject] public required GameManager GameManager { get; init; }

		[Inject] public required NavigationManager Navigation { get; init; }

		[Inject] public required AuthenticationService Authentication { get; init; }

		protected override void OnInitialized() =>
			this.GameManager.GameListUpdated += this.InvokeRender;

		private void InvokeRender(GameManager __, GameListUpdatedArgs ___) =>
			_ = this.InvokeAsync(this.StateHasChanged);

		// @todo Refactor
		private void JoinGame(IGame game)
		{
			Debug.Assert(this.Authentication.User is not null);

			bool joined;

			if (game is Solitaire solitaire)
			{
				joined = this.GameManager.TryGetOrJoin(solitaire, this.Authentication.User.AsPlayer<SolitairePlayer>());
			}
			else
			{
				throw new System.Exception($"Unknown game type: {game.GetType().Name}");
			}

			if (joined)
			{
				this.Navigation.NavigateTo($"/games/{game.Code}", true);
			}
			else
			{
				// @todo Show error
			}
		}

		public void Dispose() =>
			this.GameManager.GameListUpdated -= this.InvokeRender;
	}
}
