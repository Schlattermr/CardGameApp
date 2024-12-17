using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Xunit;
using Engines;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UnitTests
{
    public class PileTests
    {
        [Fact]
        public void DrawTopCardWhenIndexIsNotZero()
        {
            // Arrange
            var card1 = new Card
            {
                CardNumber = Engines.Number.Ten, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire
            };
            var card2 = new Card
            {
                CardNumber = Engines.Number.Nine, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire
            };
            var card3 = new Card
            {
                CardNumber = Engines.Number.Eight, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire
            };
            List<Card> cards =
            [
                card1,
                card2,
                card3
            ];

            var tableauPile = new TableauPile()
            {
                cards = cards
            };

            // Act
            tableauPile.cards.RemoveAt(0);

            // Assert
            Assert.Equal(tableauPile.TopCard(), card2);
        }

        [Fact]
        public void FoundationPileIsComplete()
        {
            // Arrange
            List<Card> cards = [];
            for (int i = 0; i < 13; i++)
            {
                cards.Add(new Card
                {
                    CardNumber = (Engines.Number)i, CardSuit = Suit.Clubs, FacingUp = false, Game = GameType.Solitaire
                });
            }

            // Act
            var foundationPile = new FoundationPile
            {
                cards = cards,
                acceptedSuit = Suit.Clubs
            };

            // Assert
            Assert.True(foundationPile.IsComplete());
        }

        [Fact]
        public void TableauPileIsEmpty()
        {
            // Arrange
            List<Card> cards = [];

            // Act
            var tableauPile = new TableauPile()
            {
                cards = cards
            };

            // Assert
            Assert.True(tableauPile.IsEmpty());
        }

        [Fact]
        public void ValidateCorrectFormOfAFacingUpPile()
        {
            // Arrange
            var card1 = new Card
            {
                CardNumber = Engines.Number.Ten, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire
            };
            var card2 = new Card
            {
                CardNumber = Engines.Number.Nine, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire
            };
            var card3 = new Card
            {
                CardNumber = Engines.Number.Eight, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire
            };
            var card4 = new Card
            {
                CardNumber = Engines.Number.Seven, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire
            };
            var card5 = new Card
            {
                CardNumber = Engines.Number.Six, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire
            };
            var card6 = new Card
            {
                CardNumber = Engines.Number.Five, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire
            };
            List<Card> cards =
            [
                card1,
                card2,
                card3,
                card4,
                card5,
                card6
            ];
            var tableauPile = new TableauPile()
            {
                cards = cards
            };

            // Assert
            Assert.True(tableauPile.ValidatePile());
        }

        [Fact]
        public void ValidatePileWithJustOneFacingUpCard()
        {
            // Arrange
            var card1 = new Card { CardNumber = Engines.Number.Seven, CardSuit = Suit.Diamonds, FacingUp = false, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Engines.Number.King, CardSuit = Suit.Hearts, FacingUp = false, Game = GameType.Solitaire };
            var card3 = new Card { CardNumber = Engines.Number.Two, CardSuit = Suit.Clubs, FacingUp = false, Game = GameType.Solitaire };
            var card4 = new Card { CardNumber = Engines.Number.Seven, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire };
            List<Card> cards =
            [
                card1,
                card2,
                card3,
                card4
            ];
            var tableauPile = new TableauPile()
            {
                cards = cards
            };

            // Assert
            Assert.True(tableauPile.ValidatePile());
        }

        [Fact]
        public void ValidatePileWithSomeFacingUpCards()
        {
            // Arrange
            var card1 = new Card { CardNumber = Engines.Number.Seven, CardSuit = Suit.Diamonds, FacingUp = false, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Engines.Number.King, CardSuit = Suit.Clubs, FacingUp = false, Game = GameType.Solitaire };
            var card3 = new Card { CardNumber = Engines.Number.Four, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            var card4 = new Card { CardNumber = Engines.Number.Three, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire };
            var card5 = new Card { CardNumber = Engines.Number.Two, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            var card6 = new Card { CardNumber = Engines.Number.Ace, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire };
            List<Card> cards =
            [
                card1,
                card2,
                card3,
                card4,
                card5,
                card6
            ];
            var tableauPile = new TableauPile()
            {
                cards = cards
            };

            // Assert
            Assert.True(tableauPile.ValidatePile());
        }

        [Fact]
        public void InvalidatePileWithNonsequentialNumber()
        {
            // Arrange
            var card1 = new Card { CardNumber = Engines.Number.Ace, CardSuit = Suit.Diamonds, FacingUp = false, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Engines.Number.Jack, CardSuit = Suit.Hearts, FacingUp = false, Game = GameType.Solitaire };
            var card3 = new Card { CardNumber = Engines.Number.Seven, CardSuit = Suit.Clubs, FacingUp = false, Game = GameType.Solitaire };
            var card4 = new Card { CardNumber = Engines.Number.Nine, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            var card5 = new Card { CardNumber = Engines.Number.Eight, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire };
            var card6 = new Card { CardNumber = Engines.Number.King, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            List<Card> cards =
            [
                card1,
                card2,
                card3,
                card4,
                card5,
                card6 // This card's value is King when it should be a seven for validity
            ];
            var tableauPile = new TableauPile()
            {
                cards = cards
            };

            // Assert
            Assert.False(tableauPile.ValidatePile());
        }

        [Fact]
        public void InvalidatePileWithWrongSuit()
        {
            // Arrange
            var card1 = new Card { CardNumber = Engines.Number.Ace, CardSuit = Suit.Diamonds, FacingUp = false, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Engines.Number.Jack, CardSuit = Suit.Hearts, FacingUp = false, Game = GameType.Solitaire };
            var card3 = new Card { CardNumber = Engines.Number.King, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            var card4 = new Card { CardNumber = Engines.Number.Queen, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire };
            var card5 = new Card { CardNumber = Engines.Number.Jack, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire };
            var card6 = new Card { CardNumber = Engines.Number.Ten, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            List<Card> cards =
            [
                card1,
                card2,
                card3,
                card4,
                card5, // This card's suit is red when it should be black
                card6 
            ];
            var tableauPile = new TableauPile()
            {
                cards = cards
            };

            // Assert
            Assert.False(tableauPile.ValidatePile());
        }
    }
}
