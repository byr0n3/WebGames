using System.Runtime.InteropServices;
using WebGames.Core.Games;

namespace WebGames.Core.Events
{
	/// <summary>
	/// Event-data that represents changes to a solitaire game.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct SolitaireStateUpdatedArgs
	{
		/// <summary>
		/// The type of update that happened.
		/// </summary>
		public readonly Solitaire.StackType From;

		/// <summary>
		/// The zero‑based index identifying the specific element that was updated in the solitaire game state.
		/// </summary>
		public readonly int FromIndex;

		/// <summary>
		/// Defines the target pile of the solitaire game that has been updated.
		/// </summary>
		public readonly Solitaire.StackType To;

		/// <summary>
		/// The zero‑based index identifying the specific element that was updated in the solitaire game state.
		/// </summary>
		public readonly int ToIndex;

		internal SolitaireStateUpdatedArgs(Solitaire.StackType from, int fromIndex, Solitaire.StackType to, int toIndex)
		{
			this.From = from;
			this.FromIndex = fromIndex;
			this.To = to;
			this.ToIndex = toIndex;
		}
	}
}
