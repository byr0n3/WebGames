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
	public struct Card : System.IEquatable<Card>
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

		/// <inheritdoc cref="Card"/>
		/// <param name="id">The card in its numeric form.</param>
		public Card(ushort id)
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

		/// <inheritdoc cref="Card"/>
		/// <param name="suit">The suit of the card.</param>
		/// <param name="rank">The rank of the card.</param>
		public Card(byte suit, byte rank)
		{
			this[0] = suit;
			this[1] = rank;
		}

		/// <inheritdoc cref="Card"/>
		/// <param name="suit">The suit of the card.</param>
		/// <param name="rank">The rank of the card.</param>
		public Card(CardSuit suit, CardRank rank)
		{
			this.Suit = suit;
			this.Rank = rank;
		}

		/// <inheritdoc />
		public readonly bool Equals(Card other) =>
			(this.Suit == other.Suit) && (this.Rank == other.Rank);

		/// <inheritdoc />
		public readonly override bool Equals(object? @object) =>
			(@object is Card other) && this.Equals(other);

		/// <inheritdoc />
		public readonly override int GetHashCode() =>
			this.@ref.GetHashCode();

		/// <inheritdoc />
		public readonly override string ToString() =>
			$"{this.Suit.ToString()} {this.Rank.ToString()}";

		/// <summary>
		/// Determines whether two <see cref="Card"/> instances represent the same card.
		/// </summary>
		/// <param name="left">The first card to compare.</param>
		/// <param name="right">The second card to compare.</param>
		/// <returns><see langword="true"/> if both cards have identical suit and rank; otherwise <see langword="false"/>.</returns>
		public static bool operator ==(Card left, Card right) =>
			left.Equals(right);

		/// <summary>
		/// Determines whether two <see cref="Card"/> instances differ in suit or rank.
		/// </summary>
		/// <param name="left">The first card to compare.</param>
		/// <param name="right">The second card to compare.</param>
		/// <returns>
		/// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> do not represent the same card;
		/// otherwise <see langword="false"/>.
		/// </returns>
		public static bool operator !=(Card left, Card right) =>
			!left.Equals(right);
	}

	internal sealed class JsonCardConverter : JsonConverter<Card>
	{
		public override Card Read(ref Utf8JsonReader reader, System.Type _, JsonSerializerOptions __) =>
			new(reader.GetUInt16());

		public override void Write(Utf8JsonWriter writer, Card value, JsonSerializerOptions _) =>
			writer.WriteNumberValue(value.Id);
	}
}
