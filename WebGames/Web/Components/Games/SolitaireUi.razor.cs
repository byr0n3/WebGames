using System;
using Microsoft.AspNetCore.Components;
using WebGames.Core;
using WebGames.Core.Events;
using WebGames.Core.Games;
using WebGames.Core.Players;

namespace WebGames.Web.Components.Games
{
	public sealed partial class SolitaireUi : ComponentBase, IDisposable
	{
		[Parameter] [EditorRequired] public required Solitaire Game { get; set; }

		[Parameter] [EditorRequired] public required SolitairePlayer Player { get; set; }

		private bool IsPlaying =>
			this.Game.State == GameState.Playing;

		private Solitaire.StackType draggedType;
		private int draggedIndex;

		protected override void OnInitialized() =>
			this.Game.StateUpdated += this.OnStateUpdate;

		// @todo Fix double rerenders (rerender when invoking UI event & rerender when state update comes in)
		// @todo Update player stats (cards moved, etc.)
		// @todo If game finished, update player stats
		private void OnStateUpdate(Solitaire __, SolitaireStateUpdatedArgs ___) =>
			_ = this.InvokeAsync(this.StateHasChanged);

		// @todo Show confirmation
		private void Restart() =>
			this.Game.Restart();

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

			this.Game.Move(Solitaire.StackType.Talon, default, Solitaire.StackType.Foundation, default);
		}

		private void TableauCardClicked(int tableauIndex)
		{
			if (!this.IsPlaying)
			{
				return;
			}

			this.Game.Move(Solitaire.StackType.Tableau, tableauIndex, Solitaire.StackType.Foundation, default);
		}

		private void OnDragStart(Solitaire.StackType type, int index)
		{
			if (!this.IsPlaying)
			{
				return;
			}

			this.draggedType = type;
			this.draggedIndex = index;
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

			this.Game.Move(this.draggedType, this.draggedIndex, type, index);

			this.draggedType = Solitaire.StackType.Invalid;
			this.draggedIndex = default;
		}

		public void Dispose() =>
			this.Game.StateUpdated -= this.OnStateUpdate;
	}
}
