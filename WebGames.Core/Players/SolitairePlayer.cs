namespace WebGames.Core.Players
{
	/// <summary>
	/// Represents a player participating in solitaire games.
	/// </summary>
	public sealed class SolitairePlayer : IPlayer
	{
		/// <inheritdoc/>
		public int Id { get; init; }

		/// <inheritdoc/>
		public string DisplayName { get; init; } = null!;

		/// <inheritdoc/>
		public bool Equals(IPlayer? other) =>
			(other is not null) && (this.Id == other.Id);

		/// <inheritdoc/>
		public override int GetHashCode() =>
			this.Id;
	}
}
