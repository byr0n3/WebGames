namespace WebGames.Core.Cards
{
	/// <summary>
	/// Represents the suit of a standard playing card.
	/// </summary>
	public enum CardSuit : byte
	{
		/// <summary>
		/// Represents a card suit that is not valid or has not been set.
		/// </summary>
		Invalid,

		/// <summary>
		/// Represents the heart suit of a standard playing card.
		/// </summary>
		Heart,

		/// <summary>
		/// Represents the diamond suit of a standard playing card.
		/// </summary>
		Diamond,

		/// <summary>
		/// Represents the spade suit of a standard playing card.
		/// </summary>
		Spade,

		/// <summary>
		/// Represents the club suit of a standard playing card.
		/// </summary>
		Club,
	}
}
