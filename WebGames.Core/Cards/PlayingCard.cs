using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebGames.Core.Cards
{
	/// <summary>
	/// Represents a standard playing card.
	/// </summary>
	[InlineArray(2)]
	[JsonConverter(typeof(JsonCardConverter))]
	public struct PlayingCard : System.IEquatable<PlayingCard>
	{
		private byte @ref;

		/// <summary>
		/// Gets or sets the suit of the card.
		/// </summary>
		public CardSuit Suit
		{
			readonly get => Unsafe.BitCast<byte, CardSuit>(this[0]);
			set => this[0] = Unsafe.BitCast<CardSuit, byte>(value);
		}

		/// <summary>
		/// Gets or sets the rank of the card.
		/// </summary>
		public CardRank Rank
		{
			readonly get => Unsafe.BitCast<byte, CardRank>(this[1]);
			set => this[1] = Unsafe.BitCast<CardRank, byte>(value);
		}

		/// <summary>
		/// Gets the numeric representation of the card as a <see cref="ushort"/>.
		/// </summary>
		public ushort Id =>
			Unsafe.ReadUnaligned<ushort>(ref this.@ref);

		/// <inheritdoc cref="PlayingCard"/>
		/// <param name="id">The card in its numeric form.</param>
		public PlayingCard(ushort id)
		{
			if (BitConverter.IsLittleEndian)
			{
				this[0] = (byte)(id & 0xff);
				this[1] = (byte)((id >> 8) & 0xff);
			}
			else
			{
				this[0] = (byte)((id >> 8) & 0xff);
				this[1] = (byte)(id & 0xff);
			}
		}

		/// <inheritdoc cref="PlayingCard"/>
		/// <param name="suit">The suit of the card.</param>
		/// <param name="rank">The rank of the card.</param>
		public PlayingCard(byte suit, byte rank)
		{
			this[0] = suit;
			this[1] = rank;
		}

		/// <inheritdoc cref="PlayingCard"/>
		/// <param name="suit">The suit of the card.</param>
		/// <param name="rank">The rank of the card.</param>
		public PlayingCard(CardSuit suit, CardRank rank)
		{
			this.Suit = suit;
			this.Rank = rank;
		}

		/// <inheritdoc />
		public readonly bool Equals(PlayingCard other) =>
			(this.Suit == other.Suit) && (this.Rank == other.Rank);

		/// <inheritdoc />
		public readonly override bool Equals(object? @object) =>
			(@object is PlayingCard other) && this.Equals(other);

		/// <inheritdoc />
		public readonly override int GetHashCode() =>
			this.@ref.GetHashCode();

		/// <inheritdoc />
		public readonly override string ToString() =>
			$"{this.Suit.ToString()} {this.Rank.ToString()}";

		/// <summary>
		/// Determines whether two <see cref="PlayingCard"/> instances represent the same card.
		/// </summary>
		/// <param name="left">The first card to compare.</param>
		/// <param name="right">The second card to compare.</param>
		/// <returns><see langword="true"/> if both cards have identical suit and rank; otherwise <see langword="false"/>.</returns>
		public static bool operator ==(PlayingCard left, PlayingCard right) =>
			left.Equals(right);

		/// <summary>
		/// Determines whether two <see cref="PlayingCard"/> instances differ in suit or rank.
		/// </summary>
		/// <param name="left">The first card to compare.</param>
		/// <param name="right">The second card to compare.</param>
		/// <returns>
		/// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> do not represent the same card;
		/// otherwise <see langword="false"/>.
		/// </returns>
		public static bool operator !=(PlayingCard left, PlayingCard right) =>
			!left.Equals(right);
	}

	internal sealed class JsonCardConverter : JsonConverter<PlayingCard>
	{
		public override PlayingCard Read(ref Utf8JsonReader reader, System.Type _, JsonSerializerOptions __) =>
			new(reader.GetUInt16());

		public override void Write(Utf8JsonWriter writer, PlayingCard value, JsonSerializerOptions _) =>
			writer.WriteNumberValue(value.Id);
	}
}
