using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using JetBrains.Annotations;

namespace WebGames.Core.Cards
{
	/// <summary>
	/// Represents a collection of one or more standard 52‑card decks.
	/// </summary>
	[MustDisposeResource]
	public sealed class CardStack : IDisposable
	{
		/// <summary>
		/// The number of cards in a standard single deck (52).
		/// </summary>
		public const int DeckSize = 52;

		private static readonly ArrayPool<PlayingCard> cardPool = ArrayPool<PlayingCard>.Create();

		private readonly PlayingCard[] cards;
		private readonly int capacity;

		private int position;

		/// <summary>
		/// Returns the card currently at the top of the stack without advancing the draw position.
		/// </summary>
		public PlayingCard Peek =>
			this.cards[this.position];

		/// <summary>
		/// The number of cards that remain in the stack before all have been drawn.
		/// </summary>
		public int Remaining =>
			this.capacity - this.position;

		/// <inheritdoc cref="CardStack"/>
		/// <param name="decks">The amount of decks to add to the stack.</param>
		public CardStack(int decks)
		{
			this.capacity = decks * CardStack.DeckSize;
			this.cards = CardStack.cardPool.Rent(this.capacity);

			this.Fill(decks);
			this.ResetAndShuffle();
		}

		// @todo Shuffle multiple times?
		/// <summary>
		/// Resets the internal position to the start of the stack and shuffles the stack.
		/// </summary>
		public void ResetAndShuffle()
		{
			this.position = 0;
			// Slice the span to not shuffle empty values into the usable array data.
			RandomNumberGenerator.Shuffle(this.cards.AsSpan(0, this.capacity));
		}

		/// <summary>
		/// Returns the next card from the deck.
		/// When the stack has been exhausted, it resets and shuffles before returning a card.
		/// </summary>
		/// <returns>The card at the current internal position.</returns>
		public PlayingCard Take()
		{
			var card = this.cards[this.position++];

			if (this.position >= this.capacity)
			{
				this.ResetAndShuffle();
			}

			return card;
		}

		/// <summary>
		/// Copies a specified number of cards from the stack into the provided destination span
		/// and advances the internal position accordingly.
		/// </summary>
		/// <param name="dst">The span into which the drawn cards are copied.</param>
		/// <param name="count">
		/// The number of cards to draw. A non‑positive value causes the method to use
		/// <paramref name="dst"/>'s length as the count.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown when <paramref name="count"/> is greater than the stack's capacity.
		/// </exception>
		public void Take(scoped Span<PlayingCard> dst, int count = 0)
		{
			if (count <= 0)
			{
				count = dst.Length;
			}

			if (count > this.capacity)
			{
				// @todo Support?
				throw new ArgumentException($"Can't draw more than {this.capacity} cards", nameof(count));
			}

			var copied = this.cards.AsSpan(this.position, count).TryCopyTo(dst);

			Debug.Assert(copied);

			this.position += count;

			if (this.position >= this.capacity)
			{
				this.ResetAndShuffle();
			}
		}

		/// <summary>
		/// Copies a specified number of cards from the stack into the provided destination list
		/// and advances the internal position accordingly.
		/// </summary>
		/// <param name="dst">The list into which the drawn cards are copied.</param>
		/// <param name="count">
		/// The number of cards to draw. A non‑positive value causes the method to use
		/// <paramref name="dst"/>'s length as the count.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown when <paramref name="count"/> is greater than the stack's capacity.
		/// </exception>
		public void Take(List<PlayingCard> dst, int count = 0)
		{
			if (count <= 0)
			{
				count = int.Min(this.Remaining, dst.Capacity);
			}

			if (count > this.capacity)
			{
				// @todo Support?
				throw new ArgumentException($"Can't draw more than {this.capacity} cards", nameof(count));
			}

			dst.AddRange(this.cards.AsSpan(this.position, count));

			this.position += count;

			if (this.position >= this.capacity)
			{
				this.ResetAndShuffle();
			}
		}

		/// <summary>
		/// Populates the stack with the specified number of standard 52‑card decks.
		/// </summary>
		/// <param name="decks">The number of decks to add to the stack.</param>
		private void Fill(int decks)
		{
			var count = 0;

			for (var _ = 0; _ < decks; _++)
			{
				for (var i = 0; i < CardStack.DeckSize; i++)
				{
					// We need to `+1` as there's an invalid fallback value in the enums.
					var suit = (byte)((i / 13) + 1);
					var rank = (byte)((i % 13) + 1);

					this.cards[count++] = new PlayingCard(suit, rank);
				}
			}
		}

		/// <inheritdoc/>
		public void Dispose() =>
			CardStack.cardPool.Return(this.cards, true);
	}
}
