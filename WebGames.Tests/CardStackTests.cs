using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using WebGames.Core.Cards;
using Xunit;
using Enum = System.Enum;

namespace WebGames.Tests
{
	public sealed class CardStackTests
	{
		[Fact]
		public void CanCreateCardDeck() =>
			CardStackTests.AssertDecks();

		[Fact]
		public void CanCreateDeckWithMultipleCards() =>
			CardStackTests.AssertDecks(5);

		[Fact]
		public void CanReshuffleDeckAfterTaking()
		{
			using var stack = new CardStack(1);

			// When the stack is fresh, it's position should be 0.
			Assert.Equal(0, GetPosition(stack));

			// Take `52 - 1` cards from the deck, so it doesn't reshuffle.
			var buffer = new PlayingCard[CardStack.DeckSize - 1];
			stack.Take(buffer);

			// The deck shouldn't have reshuffled yet.
			Assert.Equal(CardStack.DeckSize - 1, GetPosition(stack));

			// Take the last card.
			stack.Take();

			// We took the last card from the stack, it should've reshuffled now.
			Assert.Equal(0, GetPosition(stack));

			return;

			[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "position")]
			static extern ref int GetPosition(CardStack stack);
		}

		[AssertionMethod]
		private static void AssertDecks(int deckCount = 1)
		{
			using var stack = new CardStack(deckCount);

			var cards = GetCards(stack);

			// `-1` removes the `Invalid` values.
			var ranks = (Enum.GetValues<CardRank>().Length - 1) * deckCount;
			var suits = (Enum.GetValues<CardSuit>().Length - 1) * deckCount;

			// Manually check suits to make it easier to figure out what suit has missing cards.
			Assert.Equal(ranks, cards.Count(static (card) => card.Suit == CardSuit.Heart));
			Assert.Equal(ranks, cards.Count(static (card) => card.Suit == CardSuit.Diamond));
			Assert.Equal(ranks, cards.Count(static (card) => card.Suit == CardSuit.Spade));
			Assert.Equal(ranks, cards.Count(static (card) => card.Suit == CardSuit.Club));

			// Manually check ranks to make it easier to figure out what rank has missing cards.
			Assert.Equal(suits, cards.Count(static (card) => card.Rank == CardRank.Ace));
			Assert.Equal(suits, cards.Count(static (card) => card.Rank == CardRank.Two));
			Assert.Equal(suits, cards.Count(static (card) => card.Rank == CardRank.Three));
			Assert.Equal(suits, cards.Count(static (card) => card.Rank == CardRank.Four));
			Assert.Equal(suits, cards.Count(static (card) => card.Rank == CardRank.Five));
			Assert.Equal(suits, cards.Count(static (card) => card.Rank == CardRank.Six));
			Assert.Equal(suits, cards.Count(static (card) => card.Rank == CardRank.Seven));
			Assert.Equal(suits, cards.Count(static (card) => card.Rank == CardRank.Eight));
			Assert.Equal(suits, cards.Count(static (card) => card.Rank == CardRank.Nine));
			Assert.Equal(suits, cards.Count(static (card) => card.Rank == CardRank.Ten));
			Assert.Equal(suits, cards.Count(static (card) => card.Rank == CardRank.Jack));
			Assert.Equal(suits, cards.Count(static (card) => card.Rank == CardRank.Queen));
			Assert.Equal(suits, cards.Count(static (card) => card.Rank == CardRank.King));

			return;

			[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "cards")]
			static extern ref PlayingCard[] GetCards(CardStack stack);
		}
	}
}
