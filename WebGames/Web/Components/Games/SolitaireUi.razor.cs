using Microsoft.AspNetCore.Components;
using WebGames.Core.Games;
using WebGames.Core.Players;

namespace WebGames.Web.Components.Games
{
	public sealed partial class SolitaireUi : ComponentBase
	{
		[Parameter] [EditorRequired] public Solitaire Game { get; set; }

		[Parameter] [EditorRequired] public SolitairePlayer Player { get; set; }

		private Solitaire.StackType draggedType;
		private int draggedIndex;

		private void NextTalonCard() =>
			this.Game.NextTalonCard();

		private void TalonCardClicked() =>
			this.Game.Move(Solitaire.StackType.Talon, default, Solitaire.StackType.Foundation, default);

		private void TableauCardClicked(int tableauIndex) =>
			this.Game.Move(Solitaire.StackType.Tableau, tableauIndex, Solitaire.StackType.Foundation, default);

		private void OnDragStart(Solitaire.StackType type, int index)
		{
			this.draggedType = type;
			this.draggedIndex = index;
		}

		private void OnDrop(Solitaire.StackType type, int index)
		{
			if ((this.draggedType == type) && (this.draggedIndex == index))
			{
				return;
			}

			this.Game.Move(this.draggedType, this.draggedIndex, type, index);

			/*switch (this.draggedType)
			{
				case DropType.Tableau when (type is DropType.Tableau):
					this.Game.MoveTableauCardToTableau(this.draggedIndex, index);
					break;

				case DropType.Tableau when (type is DropType.Foundation):
					this.Game.MoveTableauCardToFoundation(this.draggedIndex);
					break;

				case DropType.Foundation when (type is DropType.Tableau):
					this.Game.MoveFoundationCardToTableau(this.draggedIndex, index);
					break;

				case DropType.Talon when (type is DropType.Tableau):
					this.Game.MoveTalonCardToTableau(index);
					break;

				case DropType.Talon when (type is DropType.Foundation):
					this.Game.MoveTalonCardToFoundation();
					break;
			}*/

			this.draggedType = Solitaire.StackType.Invalid;
			this.draggedIndex = default;
		}
	}
}
