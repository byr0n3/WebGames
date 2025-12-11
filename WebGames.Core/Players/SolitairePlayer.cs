namespace WebGames.Core.Players
{
	public sealed class SolitairePlayer : IPlayer
	{
		public int Id { get; init; }

		public string DisplayName { get; init; } = null!;

		public bool Equals(IPlayer? other) =>
			(other is not null) && (this.Id == other.Id);

		public override int GetHashCode() =>
			this.Id;
	}
}
