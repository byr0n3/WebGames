using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using WebGames.Core;
using WebGames.Core.Events;
using WebGames.Resources;

namespace WebGames.Web.Pages
{
	public sealed partial class Play : ComponentBase, IDisposable
	{
		[Inject] public required GameManager GameManager { get; init; }

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

		public void Dispose() =>
			this.GameManager.GameListUpdated -= this.InvokeRender;
	}
}
