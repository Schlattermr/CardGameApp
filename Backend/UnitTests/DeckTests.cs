using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Xunit;
using Engines;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UnitTests
{
    public class DeckTests
    {
        [Fact]
        public void CreateDeck_Solitaire_ShouldReturn52CardsWithoutJokers()
        {
            // Arrange
            var expectedCardCount = 52; // 52 cards + 0 jokers
            var expectedJokerCount = 0;

            // Act
            var result = new Deck();
            var cards = result.CreateDeck(GameType.Solitaire);
            result.cards = cards;
            var actualJokerCount = result.CountJokers(result.cards);

            // Assert
            Assert.Equal(expectedCardCount, result.cards.Count);
            Assert.Equal(expectedJokerCount, actualJokerCount);
            Assert.All(result.cards, card => Assert.False(card.FacingUp));
            Assert.All(result.cards, card => Assert.Equal(GameType.Solitaire, card.Game));
        }

        [Fact]
        public void CreateDeck_War_ShouldReturn54CardsIncludingJokers()
        {
            // Arrange
            const int expectedCardCount = 54; // 52 cards + 2 jokers
            const int expectedJokerCount = 2;

            // Act
            var result = new Deck();
            var cards = result.CreateDeck(GameType.War);
            result.cards = cards;
            var actualJokerCount = result.CountJokers(result.cards);

            // Assert
            Assert.Equal(expectedCardCount, result.cards.Count);
            Assert.Equal(expectedJokerCount, actualJokerCount);
            Assert.All(result.cards, card => Assert.False(card.FacingUp));
            Assert.All(result.cards, card => Assert.Equal(GameType.War, card.Game));
        }

        [Fact]
        public void CreateDeck_InvalidGameType_ShouldReturnEmptyList()
        {
            // Act
            var result = new Deck();
            var cards = result.CreateDeck((GameType)3); // An invalid game type
            result.cards = cards;
            var result2 = new Deck();
            var cards2 = result.CreateDeck((GameType)(-1)); // An invalid game type
            result2.cards = cards2;

            // Assert
            Assert.Empty(result.cards);
            Assert.Empty(result2.cards);
        }

        [Fact]
        public void AddCard_ToEmptyList_ShouldAddAllCards()
        {
            // Arrange
            var card1 = new Card { CardNumber = Engines.Number.Ace, CardSuit = Suit.Hearts, FacingUp = false, Game = GameType.War };
            var card2 = new Card { CardNumber = Engines.Number.King, CardSuit = Suit.Spades, FacingUp = false, Game = GameType.War };
            var roundCards = new List<Card>
            {
                card1,
                card2
            };

            var deck = new Deck();

            // Act
            var result = new Deck();
            result.cards = result.AddCardToDeck(roundCards);
            
            // Assert
            Assert.Equal(roundCards.Count, result.cards.Count);
            Assert.Equal(roundCards, result.cards);
        }

        [Fact]
        public void AddCard_ToNonEmptyList_ShouldAddAllCards()
        {
            // Arrange
            var card1 = new Card { CardNumber = Engines.Number.Two, CardSuit = Suit.Hearts, FacingUp = false, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Engines.Number.Jack, CardSuit = Suit.Diamonds, FacingUp = false, Game = GameType.Solitaire };
            var card3 = new Card { CardNumber = Engines.Number.Queen, CardSuit = Suit.Clubs, FacingUp = false, Game = GameType.Solitaire };
            var roundCards = new List<Card>
            {
                card2,
                card3
            };
            var userCards = new List<Card>
            {
                card1
            };

            // Act
            var result = new Deck();
            result.cards = result.AddCardToDeck(userCards);
            result.cards = result.AddCardToDeck(roundCards);

            // Assert
            Assert.Equal(3, result.cards.Count); // 1 initial card + 2 added cards
            Assert.Equal(userCards[0], result.cards[0]);
            Assert.Equal(roundCards[0], result.cards[1]);
            Assert.Equal(roundCards[1], result.cards[2]);
        }

        [Fact]
        public void AddCard_WithEmptyInputList_ShouldNotModifyCards()
        {
            // Arrange
            var roundCards = new List<Card>();

            // Act
            var result = new Deck();
            result.cards = result.AddCardToDeck(roundCards);

            // Assert
            Assert.Empty(result.cards);
        }

        [Fact]
        public void PullTopCard_FromNonEmptyDeck_ShouldReturnTopCardAndRemoveIt()
        {
            // Arrange
            var card1 = new Card { CardNumber = Engines.Number.Four, CardSuit = Suit.Clubs, FacingUp = false, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Engines.Number.Five, CardSuit = Suit.Diamonds, FacingUp = false, Game = GameType.Solitaire };
            var userCards = new List<Card>
            {
                card1,
                card2
            };

            // Act
            var result = new Deck();
            result.AddCardToDeck(userCards);
            var pulledCard = result.PullTopCard();

            // Assert
            Assert.Equal(card1, pulledCard);                           // Verify the pulled card is the top card
            Assert.True(result.cards.Count == 1);                                  // Verify only one card remains
            Assert.Equal(Engines.Number.Five, result.cards[0].CardNumber); // Verify the next card is now the top card
        }

        [Fact]
        public void PullTopCard_FromEmptyDeck_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var userCards = new List<Card>();

            // Act
            var result = new Deck();
            var exception = Assert.Throws<InvalidOperationException>(() => result.PullTopCard());

            // Assert
            Assert.Equal("No Cards in Deck", exception.Message);
        }

        [Fact]
        public void PullTopCard_DeckBecomesEmptyAfterLastCard()
        {
            // Arrange
            var lastCard = new Card { CardNumber = Engines.Number.Six, CardSuit = Suit.Diamonds, FacingUp = false, Game = GameType.Solitaire };
            var userCards = new List<Card>
            {
                lastCard
            };

            // Act
            var result = new Deck();
            result.AddCardToDeck(userCards);
            var pulledCard = result.PullTopCard();

            // Assert
            Assert.Equal(lastCard, pulledCard); // Verify the pulled card is correct
            Assert.Empty(result.cards); // Verify the deck is now empty
        }

        [Fact]
        public void FirstCardIsShuffled()
        {
            // Arrange
            var firstCard = new Card
                { CardNumber = Engines.Number.Ace, CardSuit = Suit.Clubs, FacingUp = false, Game = GameType.Solitaire };
            var result = new Deck();

            // Act
            var cards = result.Shuffle(result.CreateDeck(GameType.Solitaire));
            result.cards = cards;
            var shuffledFirstCard = result.PullTopCard();

            // Assert
            Assert.NotEqual(firstCard, shuffledFirstCard); // Ace of Clubs will never be in the top position
        }

        [Fact]
        public void EmptyDeckReturnsEmpty()
        {
            // Arrange
            var userCards = new List<Card>();

            // Act
            var result = new Deck();
            result.cards = userCards;

            var exception = Assert.Throws<InvalidOperationException>(() => result.Shuffle(userCards));

            // Assert
            Assert.Equal("No Cards in Deck", exception.Message);
        }

        [Fact]
        public void SolitaireCardsStillExistAfterShuffle()
        {
            // Arrange
            var baseDeck = new Deck();
            baseDeck.cards = baseDeck.CreateDeck(GameType.Solitaire);

            // Act
            var result = new Deck();
            result.cards = result.Shuffle(result.CreateDeck(GameType.Solitaire));

            // Assert
            Assert.Equivalent(baseDeck, result);
        }

        [Fact]
        public void WarCardsStillExistAfterShuffle()
        {
            // Arrange
            var baseDeck = new Deck();
            baseDeck.cards = baseDeck.CreateDeck(GameType.War);

            // Act
            var result = new Deck();
            result.cards = result.Shuffle(result.CreateDeck(GameType.War));

            // Assert
            Assert.Equivalent(baseDeck, result);
        }
    }
}
