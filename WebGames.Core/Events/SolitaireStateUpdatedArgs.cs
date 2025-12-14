using System.Runtime.InteropServices;

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
		public readonly SolitaireStateUpdateTarget From;

		/// <summary>
		/// Defines the target pile of the solitaire game that has been updated.
		/// </summary>
		public readonly SolitaireStateUpdateTarget To;

		/// <summary>
		/// The zero‑based index identifying the specific element that was updated in the solitaire game state.
		/// </summary>
		public readonly int Index;

		internal SolitaireStateUpdatedArgs(SolitaireStateUpdateTarget from, SolitaireStateUpdateTarget to, int index)
		{
			this.From = from;
			this.To = to;
			this.Index = index;
		}

		internal static SolitaireStateUpdatedArgs TalonCardUpdated(int index) =>
			new(SolitaireStateUpdateTarget.Talon, SolitaireStateUpdateTarget.Invalid, index);

		internal static SolitaireStateUpdatedArgs TalonCardToFoundation() =>
			new(SolitaireStateUpdateTarget.Talon, SolitaireStateUpdateTarget.Foundation, default);

		internal static SolitaireStateUpdatedArgs TableauCardToFoundation(int index) =>
			new(SolitaireStateUpdateTarget.Tableau, SolitaireStateUpdateTarget.Foundation, index);
	}

	/// <summary>
	/// Identifies the pile that has been updated during a solitaire game.
	/// </summary>
	public enum SolitaireStateUpdateTarget
	{
		/// <summary>
		/// Invalid default value.
		/// </summary>
		Invalid = -1,

		/// <summary>
		/// The talon pile was the target of the update.
		/// </summary>
		Talon,

		/// <summary>
		/// A foundation pile was the target of the update.
		/// </summary>
		Foundation,

		/// <summary>
		/// A tableau pile was the target of the update.
		/// </summary>
		Tableau,
	}
}
