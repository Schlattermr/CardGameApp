using System;
using System.Collections.Generic;
using Xunit;
using Backend.Services;

namespace Test
{
    public class DeckTests
    {
        [Fact]
        public void CreateDeck_Solitaire_ShouldReturn52CardsWithoutJokers()
        {
            var expectedCardCount = 52;
            var expectedJokerCount = 0;

            var result = new Deck();
            var cards = result.CreateDeck(GameType.Solitaire);
            result.Cards = cards;
            var actualJokerCount = result.CountJokers(result.Cards);

            Assert.Equal(expectedCardCount, result.Cards.Count);
            Assert.Equal(expectedJokerCount, actualJokerCount);
            Assert.All(result.Cards, card => Assert.False(card.FacingUp));
            Assert.All(result.Cards, card => Assert.Equal(GameType.Solitaire, card.Game));
        }

        [Fact]
        public void CreateDeck_War_ShouldReturn54CardsIncludingJokers()
        {
            const int expectedCardCount = 54;
            const int expectedJokerCount = 2;

            var result = new Deck();
            var cards = result.CreateDeck(GameType.War);
            result.Cards = cards;
            var actualJokerCount = result.CountJokers(result.Cards);

            Assert.Equal(expectedCardCount, result.Cards.Count);
            Assert.Equal(expectedJokerCount, actualJokerCount);
            Assert.All(result.Cards, card => Assert.False(card.FacingUp));
            Assert.All(result.Cards, card => Assert.Equal(GameType.War, card.Game));
        }

        [Fact]
        public void CreateDeck_InvalidGameType_ShouldReturnEmptyList()
        {
            var result = new Deck();
            var cards = result.CreateDeck((GameType)3);
            result.Cards = cards;
            var result2 = new Deck();
            var cards2 = result.CreateDeck((GameType)(-1));
            result2.Cards = cards2;

            Assert.Empty(result.Cards);
            Assert.Empty(result2.Cards);
        }

        [Fact]
        public void AddCard_ToEmptyList_ShouldAddAllCards()
        {
            var card1 = new Card { CardNumber = Number.Ace, CardSuit = Suit.Hearts, FacingUp = false, Game = GameType.War };
            var card2 = new Card { CardNumber = Number.King, CardSuit = Suit.Spades, FacingUp = false, Game = GameType.War };
            var roundCards = new List<Card> { card1, card2 };

            var result = new Deck();
            result.Cards = result.AddCardToDeck(roundCards);

            Assert.Equal(roundCards.Count, result.Cards.Count);
            Assert.Equal(roundCards, result.Cards);
        }

        [Fact]
        public void AddCard_ToNonEmptyList_ShouldAddAllCards()
        {
            var card1 = new Card { CardNumber = Number.Two, CardSuit = Suit.Hearts, FacingUp = false, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Number.Jack, CardSuit = Suit.Diamonds, FacingUp = false, Game = GameType.Solitaire };
            var card3 = new Card { CardNumber = Number.Queen, CardSuit = Suit.Clubs, FacingUp = false, Game = GameType.Solitaire };
            var roundCards = new List<Card> { card2, card3 };
            var userCards = new List<Card> { card1 };

            var result = new Deck();
            result.Cards = result.AddCardToDeck(userCards);
            result.Cards = result.AddCardToDeck(roundCards);

            Assert.Equal(3, result.Cards.Count);
            Assert.Equal(userCards[0], result.Cards[0]);
            Assert.Equal(roundCards[0], result.Cards[1]);
            Assert.Equal(roundCards[1], result.Cards[2]);
        }

        [Fact]
        public void AddCard_WithEmptyInputList_ShouldNotModifyCards()
        {
            var roundCards = new List<Card>();

            var result = new Deck();
            result.Cards = result.AddCardToDeck(roundCards);

            Assert.Empty(result.Cards);
        }

        [Fact]
        public void PullTopCard_FromNonEmptyDeck_ShouldReturnTopCardAndRemoveIt()
        {
            var card1 = new Card { CardNumber = Number.Four, CardSuit = Suit.Clubs, FacingUp = false, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Number.Five, CardSuit = Suit.Diamonds, FacingUp = false, Game = GameType.Solitaire };
            var userCards = new List<Card> { card1, card2 };

            var result = new Deck();
            result.AddCardToDeck(userCards);
            var pulledCard = result.PullTopCard();

            Assert.Equal(card1, pulledCard);
            Assert.True(result.Cards.Count == 1);
            Assert.Equal(Number.Five, result.Cards[0].CardNumber);
        }

        [Fact]
        public void PullTopCard_FromEmptyDeck_ShouldThrowInvalidOperationException()
        {
            var result = new Deck();
            var exception = Assert.Throws<InvalidOperationException>(() => result.PullTopCard());

            Assert.Equal("No Cards in Deck", exception.Message);
        }

        [Fact]
        public void PullTopCard_DeckBecomesEmptyAfterLastCard()
        {
            var lastCard = new Card { CardNumber = Number.Six, CardSuit = Suit.Diamonds, FacingUp = false, Game = GameType.Solitaire };
            var userCards = new List<Card> { lastCard };

            var result = new Deck();
            result.AddCardToDeck(userCards);
            var pulledCard = result.PullTopCard();

            Assert.Equal(lastCard, pulledCard);
            Assert.Empty(result.Cards);
        }

        [Fact]
        public void FirstCardIsShuffled()
        {
            var firstCard = new Card { CardNumber = Number.Ace, CardSuit = Suit.Clubs, FacingUp = false, Game = GameType.Solitaire };
            var result = new Deck();

            var cards = result.Shuffle(result.CreateDeck(GameType.Solitaire));
            result.Cards = cards;
            var shuffledFirstCard = result.PullTopCard();

            Assert.NotEqual(firstCard, shuffledFirstCard);
        }

        [Fact]
        public void EmptyDeckReturnsEmpty()
        {
            var userCards = new List<Card>();

            var result = new Deck();
            result.Cards = userCards;

            var exception = Assert.Throws<InvalidOperationException>(() => result.Shuffle(userCards));

            Assert.Equal("No Cards in Deck", exception.Message);
        }

        [Fact]
        public void SolitaireCardsStillExistAfterShuffle()
        {
            var baseDeck = new Deck();
            baseDeck.Cards = baseDeck.CreateDeck(GameType.Solitaire);

            var result = new Deck();
            result.Cards = result.Shuffle(result.CreateDeck(GameType.Solitaire));

            Assert.Equivalent(baseDeck, result);
        }

        [Fact]
        public void WarCardsStillExistAfterShuffle()
        {
            var baseDeck = new Deck();
            baseDeck.Cards = baseDeck.CreateDeck(GameType.War);

            var result = new Deck();
            result.Cards = result.Shuffle(result.CreateDeck(GameType.War));

            Assert.Equivalent(baseDeck, result);
        }
    }
}
