using Backend.Models.Domain;
using Backend.Models.Enums;
using Backend.Services;

namespace Test
{
    public class BlackjackTests
    {
        private static Card MakeCard(Number number, Suit suit = Suit.Clubs, bool facingUp = true)
        {
            return new Card
            {
                CardNumber = number,
                CardSuit = suit,
                FacingUp = facingUp,
                Game = GameType.Blackjack
            };
        }

        [Fact]
        public void GetHandValue_NumberCards_SumsFaceValue()
        {
            var hand = new List<Card> { MakeCard(Number.Five), MakeCard(Number.Six) };

            Assert.Equal(11, BlackjackRules.GetHandValue(hand));
        }

        [Fact]
        public void GetHandValue_FaceCards_CountAsTen()
        {
            var hand = new List<Card> { MakeCard(Number.King), MakeCard(Number.Queen) };

            Assert.Equal(20, BlackjackRules.GetHandValue(hand));
        }

        [Fact]
        public void GetHandValue_AceCountsAsEleven_WhenNotBust()
        {
            var hand = new List<Card> { MakeCard(Number.Ace), MakeCard(Number.Nine) };

            Assert.Equal(20, BlackjackRules.GetHandValue(hand));
        }

        [Fact]
        public void GetHandValue_AceCountsAsOne_WhenElevenWouldBust()
        {
            var hand = new List<Card> { MakeCard(Number.Ace), MakeCard(Number.King), MakeCard(Number.Five) };

            Assert.Equal(16, BlackjackRules.GetHandValue(hand));
        }

        [Fact]
        public void GetHandValue_MultipleAces_OnlyOneCountsAsEleven()
        {
            var hand = new List<Card> { MakeCard(Number.Ace), MakeCard(Number.Ace), MakeCard(Number.Nine) };

            Assert.Equal(21, BlackjackRules.GetHandValue(hand));
        }

        [Fact]
        public void IsBust_HandOverTwentyOne_ReturnsTrue()
        {
            var hand = new List<Card> { MakeCard(Number.King), MakeCard(Number.Queen), MakeCard(Number.Two) };

            Assert.True(BlackjackRules.IsBust(hand));
        }

        [Fact]
        public void IsBust_HandTwentyOneOrUnder_ReturnsFalse()
        {
            var hand = new List<Card> { MakeCard(Number.King), MakeCard(Number.Ace) };

            Assert.False(BlackjackRules.IsBust(hand));
        }

        [Fact]
        public void IsBlackjack_TwoCardTwentyOne_ReturnsTrue()
        {
            var hand = new List<Card> { MakeCard(Number.Ace), MakeCard(Number.King) };

            Assert.True(BlackjackRules.IsBlackjack(hand));
        }

        [Fact]
        public void IsBlackjack_ThreeCardTwentyOne_ReturnsFalse()
        {
            var hand = new List<Card> { MakeCard(Number.Seven), MakeCard(Number.Seven), MakeCard(Number.Seven) };

            Assert.False(BlackjackRules.IsBlackjack(hand));
        }

        [Fact]
        public void StartRound_DealsTwoCardsEachWithDealerHoleCardFaceDown()
        {
            var rules = new BlackjackRules();

            rules.StartRound();

            Assert.Equal(2, rules.PlayerHand.Count);
            Assert.Equal(2, rules.DealerHand.Count);
            Assert.All(rules.PlayerHand, card => Assert.True(card.FacingUp));
            Assert.True(rules.DealerHand[0].FacingUp);

            if (!rules.RoundOver)
            {
                Assert.False(rules.DealerHand[1].FacingUp);
            }
        }

        [Fact]
        public void Hit_AddsCardToPlayerHand()
        {
            var rules = new BlackjackRules();
            rules.StartRound();
            var initialCount = rules.PlayerHand.Count;

            if (!rules.RoundOver)
            {
                rules.Hit();
                Assert.Equal(initialCount + 1, rules.PlayerHand.Count);
            }
        }

        [Fact]
        public void Hit_WhenRoundOver_ThrowsInvalidOperationException()
        {
            var rules = new BlackjackRules();
            rules.StartRound();

            while (!rules.RoundOver)
            {
                rules.Hit();
            }

            Assert.Throws<InvalidOperationException>(() => rules.Hit());
        }

        [Fact]
        public void Stand_RevealsDealerHoleCardAndEndsRound()
        {
            var rules = new BlackjackRules();
            rules.StartRound();

            if (!rules.RoundOver)
            {
                rules.Stand();

                Assert.True(rules.RoundOver);
                Assert.All(rules.DealerHand, card => Assert.True(card.FacingUp));
            }
        }

        [Fact]
        public void Stand_DealerDrawsUntilAtLeastSeventeen()
        {
            var rules = new BlackjackRules();
            rules.StartRound();

            if (!rules.RoundOver)
            {
                rules.Stand();

                Assert.True(BlackjackRules.GetHandValue(rules.DealerHand) >= 17 || BlackjackRules.IsBust(rules.DealerHand));
            }
        }

        [Fact]
        public void Stand_WhenRoundOver_ThrowsInvalidOperationException()
        {
            var rules = new BlackjackRules();
            rules.StartRound();

            while (!rules.RoundOver)
            {
                rules.Hit();
            }

            Assert.Throws<InvalidOperationException>(() => rules.Stand());
        }

        [Fact]
        public void GetResult_BeforeRoundOver_ReturnsInProgress()
        {
            var rules = new BlackjackRules();
            rules.StartRound();

            if (!rules.RoundOver)
            {
                Assert.Equal("In Progress", rules.GetResult());
            }
        }

        [Fact]
        public void GetResult_PlayerBust_ReturnsDealerWinsMessage()
        {
            var rules = new BlackjackRules();
            rules.StartRound();

            while (!rules.RoundOver)
            {
                rules.Hit();
            }

            if (BlackjackRules.IsBust(rules.PlayerHand))
            {
                Assert.Equal("Dealer Wins - Player Bust", rules.GetResult());
            }
        }
    }
}
