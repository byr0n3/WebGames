using WebGames.Core.Cards;
using Xunit;

namespace WebGames.Tests
{
	public sealed class CardTests
	{
		private static readonly PlayingCard diamondJack = new(CardSuit.Diamond, CardRank.Jack);
		private const ushort diamondJackId = 2818;

		[Fact]
		public void CanSerializeCards()
		{
			var cardId = CardTests.diamondJack.Id;

			Assert.Equal(CardTests.diamondJackId, cardId);
		}

		[Fact]
		public void CanDeserializeCards()
		{
			var card = new PlayingCard(CardTests.diamondJackId);

			Assert.Equal(CardTests.diamondJack, card);
		}
	}
}
