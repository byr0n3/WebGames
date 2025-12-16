using System;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WebGames.Core;
using WebGames.Core.Events;
using WebGames.Core.Games;
using WebGames.Core.Players;
using WebGames.Extensions;

namespace WebGames.Web.Components.Games
{
	public sealed partial class SolitaireUi : ComponentBase, IDisposable
	{
		[Inject] public required IJSRuntime Js { get; init; }

		[Parameter] [EditorRequired] public required Solitaire Game { get; set; }

		[Parameter] [EditorRequired] public required SolitairePlayer Player { get; set; }

		private bool IsPlaying =>
			this.Game.State == GameState.Playing;

		private Solitaire.StackType draggedType;
		private int draggedIndex;
		private int draggedCardIndex;

		protected override void OnInitialized()
		{
			this.Game.GameUpdated += this.OnGameUpdated;
			this.Game.StateUpdated += this.OnStateUpdate;
		}

		private void OnGameUpdated(IGame game, GameUpdatedArgs args)
		{
			if (args.Type == GameUpdateType.StateUpdated)
			{
				if (game.State == GameState.Playing)
				{
					// @todo Update player games stats
				}

				if (game.State == GameState.Done)
				{
					_ = this.InvokeAsync(() => this.Js.ConfettiAsync(new ConfettiConfig
					{
						ParticleCount = 100,
						Spread = 70,
						Origin = new ConfettiConfig.ConfettiOrigin
						{
							Y = 0.6f,
						},
					}).AsTask());

					// @todo Update player win stats
				}
			}
		}

		// @todo Fix double rerenders (rerender when invoking UI event & rerender when state update comes in)
		// @todo Update player stats (cards moved, etc.)
		private void OnStateUpdate(Solitaire __, SolitaireStateUpdatedArgs ___) =>
			_ = this.InvokeAsync(this.StateHasChanged);

		// @todo Show confirmation
		private void Restart() =>
			this.Game.Restart();

		private void Finish()
		{
			if (!this.IsPlaying)
			{
				return;
			}

			this.Game.AutoFinish();
		}

		private void NextTalonCard()
		{
			if (!this.IsPlaying)
			{
				return;
			}

			this.Game.NextTalonCard();
		}

		private void TalonCardClicked()
		{
			if (!this.IsPlaying)
			{
				return;
			}

			this.Game.Move(Solitaire.StackType.Talon, default, default, Solitaire.StackType.Foundation, default);
		}

		private void TableauCardClicked(int tableauIndex)
		{
			if (!this.IsPlaying)
			{
				return;
			}

			this.Game.Move(Solitaire.StackType.Tableau, tableauIndex, default, Solitaire.StackType.Foundation, default);
		}

		private void OnDragStart(Solitaire.StackType type, int index, int cardIndex = -1)
		{
			if (!this.IsPlaying)
			{
				return;
			}

			this.draggedType = type;
			this.draggedIndex = index;
			this.draggedCardIndex = cardIndex;
		}

		private void OnDrop(Solitaire.StackType type, int index)
		{
			if (!this.IsPlaying || (this.draggedType == Solitaire.StackType.Invalid))
			{
				return;
			}

			if ((this.draggedType == type) && (this.draggedIndex == index))
			{
				return;
			}

			this.Game.Move(this.draggedType, this.draggedIndex, this.draggedCardIndex, type, index);

			this.draggedType = Solitaire.StackType.Invalid;
			this.draggedIndex = default;
		}

		public void Dispose()
		{
			this.Game.GameUpdated -= this.OnGameUpdated;
			this.Game.StateUpdated -= this.OnStateUpdate;
		}
	}
}
