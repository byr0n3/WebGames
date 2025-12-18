using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using WebGames.Core;
using WebGames.Core.Events;
using WebGames.Core.Games;
using WebGames.Core.Players;
using WebGames.Extensions;
using WebGames.Resources;
using WebGames.Services;

namespace WebGames.Web.Pages
{
	public sealed partial class Play : ComponentBase, IDisposable
	{
		[Inject] public required GameManager GameManager { get; init; }

		[Inject] public required NavigationManager Navigation { get; init; }

		[Inject] public required AuthenticationService Authentication { get; init; }

		[Inject] public required IStringLocalizer<PlayLocalization> Localizer { get; init; }

		[Inject] public required IStringLocalizer<GameLocalization> GameLocalizer { get; init; }

		private IGame[]? games;

		protected override void OnInitialized()
		{
			this.games = this.GameManager.Games.ToArray();

			this.GameManager.GameListUpdated += this.InvokeRender;
		}

		private void InvokeRender(GameManager __, GameListUpdatedArgs args)
		{
			this.games = this.GameManager.Games.ToArray();

			_ = this.InvokeAsync(this.StateHasChanged);
		}

		private void JoinGame(IGame game)
		{
			Debug.Assert(this.Authentication.User is not null);

			bool joined;

			// @todo Refactor
			if (game is Solitaire solitaire)
			{
				joined = this.GameManager.TryGetOrJoin(solitaire, this.Authentication.User.AsPlayer<SolitairePlayer>());
			}
			else
			{
				throw new System.Exception($"Unknown game type: {game.GetType().Name}");
			}

			Debug.Assert(joined);

			this.Navigation.NavigateTo($"/play/{game.Code}", true);
		}

		public void Dispose() =>
			this.GameManager.GameListUpdated -= this.InvokeRender;
	}
}
