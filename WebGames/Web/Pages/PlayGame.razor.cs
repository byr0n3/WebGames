using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using WebGames.Core;
using WebGames.Core.Events;
using WebGames.Core.Players;
using WebGames.Extensions;
using WebGames.Services;

namespace WebGames.Web.Pages
{
	public sealed partial class PlayGame : ComponentBase, IDisposable
	{
		[Inject] public required IJSRuntime Js { get; init; }

		[Inject] public required GameManager GameManager { get; init; }

		[Inject] public required NavigationManager Navigation { get; init; }

		[Inject] public required AuthenticationService Authentication { get; init; }

		[Inject] public required IStringLocalizer<PlayGameLocalization> Localizer { get; init; }

		[Parameter] public required string Code { get; init; }

		private bool spectating;
		private IPlayer player = null!;
		private IGame? game;

		protected override void OnInitialized()
		{
			this.spectating = this.Navigation.Uri.EndsWith("/spectate", StringComparison.OrdinalIgnoreCase);

			bool success;

			if (this.spectating)
			{
				success = this.GameManager.TryGet(this.Code, out this.game);
			}
			else
			{
				Debug.Assert(this.Authentication.User is not null);

				// @todo Based on game type
				this.player = this.Authentication.User.AsPlayer<SolitairePlayer>();

				success = this.GameManager.TryJoin(this.Code, this.player, out this.game);
			}

			// Game not found, unable to join, not allowed to spectate.
			if ((this.game is null) || !success || (this.spectating && !this.game.Configuration.AllowSpectators))
			{
				this.Navigation.NavigateTo("/play", true);
				return;
			}

			this.game.GameUpdated += this.OnGameUpdated;
		}

		private void OnGameUpdated(IGame game, GameUpdatedArgs args)
		{
			if ((args.Type == GameUpdateType.PlayerLeft) && (game.CurrentPlayers.Count == 0))
			{
				this.Navigation.NavigateTo("/play", true);
				return;
			}

			_ = this.InvokeAsync(this.StateHasChanged);
		}

		private async Task LeaveAsync()
		{
			if (!this.spectating)
			{
				var confirmed = await this.Js.ConfirmAsync("Are you sure you want to leave?");

				if (!confirmed)
				{
					return;
				}
			}

			this.Navigation.NavigateTo("/play", true);
		}

		public void Dispose()
		{
			if (this.game is null)
			{
				return;
			}

			this.game.GameUpdated -= this.OnGameUpdated;

			if (this.spectating)
			{
				return;
			}

			this.GameManager.Leave(this.game, this.player);
		}
	}
}
