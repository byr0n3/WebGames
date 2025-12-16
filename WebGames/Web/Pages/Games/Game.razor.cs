using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using WebGames.Core;
using WebGames.Core.Events;
using WebGames.Core.Players;
using WebGames.Extensions;
using WebGames.Services;

namespace WebGames.Web.Pages.Games
{
	public sealed partial class Game : ComponentBase, IDisposable
	{
		[Inject] public required GameManager GameManager { get; init; }

		[Inject] public required NavigationManager Navigation { get; init; }

		[Inject] public required AuthenticationService Authentication { get; init; }

		[Inject] public required IStringLocalizer<GameLocalization> Localizer { get; init; }

		[Parameter] public required string Code { get; init; }

		private IPlayer player = null!;
		private IGame? game;

		protected override void OnInitialized()
		{
			Debug.Assert(this.Authentication.User is not null);

			// @todo Based on game type
			this.player = this.Authentication.User.AsPlayer<SolitairePlayer>();

			if (!this.GameManager.TryGetOrJoin(this.Code, this.player, out this.game))
			{
				this.Navigation.NavigateTo("/games", true);
				return;
			}

			this.game.GameUpdated += this.OnGameUpdated;
		}

		private void OnGameUpdated(IGame __, GameUpdatedArgs ___) =>
			_ = this.InvokeAsync(this.StateHasChanged);

		// @todo Refactor
		public void Dispose()
		{
			if (this.game is null)
			{
				return;
			}

			this.game.GameUpdated -= this.OnGameUpdated;
			this.GameManager.Leave(this.game, this.player);
		}
	}
}
