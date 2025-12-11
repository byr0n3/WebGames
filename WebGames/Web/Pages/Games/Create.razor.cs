using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using WebGames.Core;
using WebGames.Core.Games;
using WebGames.Core.Players;
using WebGames.Extensions;
using WebGames.Models.Requests;
using WebGames.Services;

namespace WebGames.Web.Pages.Games
{
	public sealed partial class Create : ComponentBase
	{
		private const string formName = nameof(Create);

		[Inject] public required GameManager GameManager { get; init; }

		[Inject] public required NavigationManager Navigation { get; init; }

		[Inject] public required AuthenticationService Authentication { get; init; }

		[SupplyParameterFromForm(FormName = Create.formName)]
		private CreateGameModel Model { get; set; } = new();

		private void CreateGame()
		{
			Debug.Assert(this.Authentication.User is not null);

			var defaultConfiguration = (this.Model.Type) switch
			{
				GameType.Solitaire => Solitaire.DefaultConfiguration,
				_                  => throw new System.Exception($"Unknown game type: {this.Model.Type}"),
			};

			// @todo More configurable options
			var configuration = defaultConfiguration with
			{
				Visibility = this.Model.Visibility,
			};

			var player = (this.Model.Type) switch
			{
				GameType.Solitaire => this.Authentication.User.AsPlayer<SolitairePlayer>(),
				_                  => throw new System.Exception($"Unknown game type: {this.Model.Type}"),
			};

			var game = (this.Model.Type) switch
			{
				GameType.Solitaire => this.GameManager.Create<Solitaire>(configuration, player),
				_                  => throw new System.Exception($"Unknown game type: {this.Model.Type}"),
			};

			this.Navigation.NavigateTo($"/games/{game.Code}", true);
		}
	}
}
