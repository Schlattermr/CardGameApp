using Backend.Services;

namespace Test
{
    public class SolitaireTests
    {
        [Fact]
        public void MoveCorrectNormalCardToFoundation()
        {
            // Arrange
            var testRules = new SolitaireRules();

            List<Card> foundation = [];
            for (int i = 0; i < 7; i++)
            {
                foundation.Add(new Card { CardNumber = (Number)i, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire });
            }
            var foundationPile = new FoundationPile()
            {
                cards = foundation,
                acceptedSuit = Suit.Clubs
            };

            List<Card> tableau = [];
            var selectedCard = new Card { CardNumber = Number.Seven, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            tableau.Add(selectedCard);
            var tableauPile = new TableauPile()
            {
                cards = tableau
            };

            // Act
            testRules.MoveToFoundation(selectedCard, tableauPile, foundationPile);

            // Assert
            Assert.Empty(tableau);
            Assert.Contains(selectedCard, foundationPile.cards);
        }

        [Fact]
        public void MoveCorrectAceCardToEmptyFoundation()
        {
            // Arrange
            var testRules = new SolitaireRules();

            List<Card> foundation = [];
            var foundationPile = new FoundationPile()
            {
                cards = foundation,
                acceptedSuit = Suit.Hearts
            };

            List<Card> tableau = [];
            var selectedCard = new Card { CardNumber = Number.Ace, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire };
            tableau.Add(selectedCard);
            var tableauPile = new TableauPile()
            {
                cards = tableau
            };

            // Act
            testRules.MoveToFoundation(selectedCard, tableauPile, foundationPile);

            // Assert
            Assert.Empty(tableau);
            Assert.NotEmpty(foundation);
            Assert.Contains(selectedCard, foundationPile.cards);
        }

        [Fact]
        public void FailToMoveCardToFoundationByGreaterNumber() // Attempts to move a card into a foundation with a lesser value than the last card
        {
            // Arrange
            var testRules = new SolitaireRules();

            List<Card> foundation = [];
            for (int i = 0; i < 4; i++)
            {
                foundation.Add(new Card { CardNumber = (Number)i, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire });
            }
            var foundationPile = new FoundationPile()
            {
                cards = foundation,
                acceptedSuit = Suit.Hearts
            };

            List<Card> tableau = [];
            var selectedCard = new Card { CardNumber = Number.Ten, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            tableau.Add(selectedCard);
            var tableauPile = new TableauPile()
            {
                cards = tableau
            };

            // Act
            testRules.MoveToFoundation(selectedCard, tableauPile, foundationPile);

            // Assert
            Assert.NotEmpty(tableau);
            Assert.DoesNotContain(selectedCard, foundationPile.cards);
        }

        [Fact]
        public void FailToMoveCardToFoundationByLesserNumber() // Attempts to move a card into a foundation with a lesser value than the last card
        {
            // Arrange
            var testRules = new SolitaireRules();

            List<Card> foundation = [];
            for (int i = 0; i < 7; i++)
            {
                foundation.Add(new Card { CardNumber = (Number)i, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire });
            }
            var foundationPile = new FoundationPile()
            {
                cards = foundation,
                acceptedSuit = Suit.Hearts
            };

            List<Card> tableau = [];
            var selectedCard = new Card { CardNumber = Number.Four, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            tableau.Add(selectedCard);
            var tableauPile = new TableauPile()
            {
                cards = tableau
            };

            // Act
            testRules.MoveToFoundation(selectedCard, tableauPile, foundationPile);

            // Assert
            Assert.NotEmpty(tableau);
            Assert.DoesNotContain(selectedCard, foundationPile.cards);
        }

        [Fact]
        public void FailToMoveNonLastCardToFoundation() // Attempts to move a card that's not in the last position
        {
            // Arrange
            var testRules = new SolitaireRules();

            List<Card> foundation = [];
            for (int i = 0; i < 5; i++)
            {
                foundation.Add(new Card { CardNumber = (Number)i, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire });
            }
            var foundationPile = new FoundationPile()
            {
                cards = foundation,
                acceptedSuit = Suit.Diamonds
            };

            var selectedCard = new Card { CardNumber = Number.Four, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            var extraCard = new Card { CardNumber = Number.Ace, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire };
            List<Card> cards =
            [
                selectedCard,
                extraCard
            ];
            var pile = new Pile()
            {
                cards = cards
            };

            // Act
            testRules.MoveToFoundation(selectedCard, pile, foundationPile);

            // Assert
            Assert.Equal(2, pile.Count());
            Assert.DoesNotContain(selectedCard, foundationPile.cards);
        }

        [Fact]
        public void FailToMoveCardToFoundationSuit() // Attempts to move a card into a foundation with a differing suit
        {
            // Arrange
            var testRules = new SolitaireRules();

            List<Card> foundation = [];
            for (int i = 0; i < 10; i++)
            {
                foundation.Add(new Card { CardNumber = (Number)i, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire });
            }

            var foundationPile = new FoundationPile()
            {
                cards = foundation,
                acceptedSuit = Suit.Hearts
            };

            List<Card> tableau = [];
            var selectedCard = new Card { CardNumber = Number.Ten, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            tableau.Add(selectedCard);
            var tableauPile = new TableauPile()
            {
                cards = tableau
            };

            // Act
            testRules.MoveToFoundation(selectedCard, tableauPile, foundationPile);

            // Assert
            Assert.NotEmpty(tableau);
            Assert.DoesNotContain(selectedCard, foundationPile.cards);
        }

        [Fact]
        public void MoveCorrectCardToEmptyTableau()
        {
            // Arrange
            var testRules = new SolitaireRules();

            List<Card> emptyTableau = [];
            var emptyTableauPile = new TableauPile()
            {
                cards = emptyTableau
            };

            List<Card> tableau = [];
            var selectedCard = new Card { CardNumber = Number.King, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            tableau.Add(selectedCard);
            var tableauPile = new TableauPile()
            {
                cards = tableau
            };

            // Act
            testRules.MoveToTableau(selectedCard, tableauPile, emptyTableauPile);

            // Assert
            Assert.False(emptyTableauPile.IsEmpty());
            Assert.True(tableauPile.IsEmpty());
            Assert.Contains(selectedCard, emptyTableauPile.cards);
        }

        [Fact]
        public void MoveWholePileToEmptyTableau()
        {
            // Arrange
            var testRules = new SolitaireRules();

            List<Card> emptyTableau = [];
            var emptyTableauPile = new TableauPile()
            {
                cards = emptyTableau
            };

            var card1 = new Card { CardNumber = Number.King, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Number.Queen, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire };
            var card3 = new Card { CardNumber = Number.Jack, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            var card4 = new Card { CardNumber = Number.Ten, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire };
            List<Card> tableau =
            [
                card1,
                card2,
                card3,
                card4

            ];
            var tableauPile = new TableauPile()
            {
                cards = tableau
            };

            // Act
            testRules.MoveToTableau(card1, tableauPile, emptyTableauPile);

            // Assert
            Assert.Equal(0, tableauPile.Count());
            Assert.Equal(4, emptyTableauPile.Count());
            Assert.Contains(card1, emptyTableauPile.cards);
            Assert.True(tableauPile.ValidatePile());
        }

        [Fact]
        public void MoveLastCardFromDiscardToEmptyTableau()
        {
            // Arrange
            var testRules = new SolitaireRules();

            List<Card> emptyTableau = [];
            var emptyTableauPile = new TableauPile()
            {
                cards = emptyTableau
            };

            var card1 = new Card { CardNumber = Number.Ace, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Number.Jack, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            var card3 = new Card { CardNumber = Number.Seven, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            var card4 = new Card { CardNumber = Number.King, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire };
            List<Card> cards =
            [
                card1,
                card2,
                card3,
                card4

            ];
            var discard = new Pile()
            {
                cards = cards
            };

            // Act
            testRules.MoveToTableau(card4, discard, emptyTableauPile);

            // Assert
            Assert.Equal(1, emptyTableauPile.Count());
            Assert.Equal(3, discard.Count());
            Assert.Contains(card4, emptyTableauPile.cards);
        }

        [Fact]
        public void FailToMoveNonLastCardFromDiscardToEmptyTableau()
        {
            // Arrange
            var testRules = new SolitaireRules();

            List<Card> emptyTableau = [];
            var emptyTableauPile = new TableauPile()
            {
                cards = emptyTableau
            };

            var card1 = new Card { CardNumber = Number.King, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Number.Nine, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            var card3 = new Card { CardNumber = Number.Five, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            var card4 = new Card { CardNumber = Number.King, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire };
            List<Card> cards =
            [
                card1,
                card2,
                card3,
                card4
            ];
            var discard = new Pile()
            {
                cards = cards
            };

            // Act
            testRules.MoveToTableau(card1, discard, emptyTableauPile);

            // Assert
            Assert.True(emptyTableauPile.IsEmpty());
            Assert.Equal(4, discard.Count());
        }

        [Fact]
        public void FailToMoveCardToEmptyTableau()
        {
            // Arrange
            var testRules = new SolitaireRules();

            List<Card> emptyTableau = [];
            var emptyTableauPile = new TableauPile()
            {
                cards = emptyTableau
            };

            List<Card> tableau = [];
            var selectedCard = new Card { CardNumber = Number.Four, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            tableau.Add(selectedCard);
            var tableauPile = new TableauPile()
            {
                cards = tableau
            };

            // Act
            testRules.MoveToTableau(selectedCard, tableauPile, emptyTableauPile);

            // Assert
            Assert.True(emptyTableauPile.IsEmpty());
            Assert.False(tableauPile.IsEmpty());
            Assert.DoesNotContain(selectedCard, emptyTableauPile.cards);
        }

        [Fact]
        public void MoveSingleCardToOccupiedTableau()
        {
            // Arrange
            var testRules = new SolitaireRules();

            var selectedCard = new Card { CardNumber = Number.Eight, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire };
            List<Card> chosenTableau =
            [
                selectedCard
            ];
            var chosenTableauPile = new TableauPile()
            {
                cards = chosenTableau
            };

            var card1 = new Card { CardNumber = Number.Jack, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Number.Ten, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire };
            var card3 = new Card { CardNumber = Number.Nine, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            List<Card> addingTableau =
            [
                card1,
                card2,
                card3
            ];
            var addingTableauPile = new TableauPile()
            {
                cards = addingTableau
            };

            // Act
            testRules.MoveToTableau(selectedCard, chosenTableauPile, addingTableauPile);

            // Assert
            Assert.True(chosenTableauPile.IsEmpty());
            Assert.True(addingTableauPile.ValidatePile());
            Assert.Equal(4, addingTableauPile.Count());
        }

        [Fact]
        public void MovePartialPileToOccupiedTableau()
        {
            // Arrange
            var testRules = new SolitaireRules();

            var card1 = new Card { CardNumber = Number.Eight, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Number.Seven, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            var card3 = new Card { CardNumber = Number.Six, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire };
            List<Card> chosenTableau =
            [
                card1,
                card2,
                card3
            ];
            var chosenTableauPile = new TableauPile()
            {
                cards = chosenTableau
            };

            var card4 = new Card { CardNumber = Number.Jack, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            var card5 = new Card { CardNumber = Number.Ten, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire };
            var card6 = new Card { CardNumber = Number.Nine, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            List<Card> addingTableau =
            [
                card4,
                card5,
                card6
            ];
            var addingTableauPile = new TableauPile()
            {
                cards = addingTableau
            };

            // Act
            testRules.MoveToTableau(card1, chosenTableauPile, addingTableauPile);

            // Assert
            Assert.True(chosenTableauPile.IsEmpty());
            Assert.True(addingTableauPile.ValidatePile());
            Assert.Equal(6, addingTableauPile.Count());
        }

        [Fact]
        public void MoveLastTwoCardsOfPartialPileToOccupiedTableau()
        {
            // Arrange
            var testRules = new SolitaireRules();

            var card1 = new Card { CardNumber = Number.Four, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Number.Three, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            var card3 = new Card { CardNumber = Number.Two, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire };
            var card4 = new Card { CardNumber = Number.Ace, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            List<Card> chosenTableau =
            [
                card1,
                card2,
                card3,
                card4
            ];
            var chosenTableauPile = new TableauPile()
            {
                cards = chosenTableau
            };

            var card5 = new Card { CardNumber = Number.Four, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire };
            var card6 = new Card { CardNumber = Number.Three, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            List<Card> addingTableau =
            [
                card5,
                card6
            ];
            var addingTableauPile = new TableauPile()
            {
                cards = addingTableau
            };

            // Act
            testRules.MoveToTableau(card3, chosenTableauPile, addingTableauPile);

            // Assert
            Assert.True(addingTableauPile.ValidatePile());
            Assert.Equal(4, addingTableauPile.Count());
            Assert.Equal(2, chosenTableauPile.Count());
        }

        [Fact]
        public void MoveLastCardFromDiscardToOccupiedTableau()
        {
            // Arrange
            var testRules = new SolitaireRules();

            var card1 = new Card { CardNumber = Number.Four, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Number.Nine, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire };
            var card3 = new Card { CardNumber = Number.Ten, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire };
            var card4 = new Card { CardNumber = Number.Two, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            List<Card> cards =
            [
                card1,
                card2,
                card3,
                card4
            ];
            var discard = new TableauPile()
            {
                cards = cards
            };

            var card5 = new Card { CardNumber = Number.Four, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            var card6 = new Card { CardNumber = Number.Three, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire };
            List<Card> addingTableau =
            [
                card5,
                card6
            ];
            var addingTableauPile = new TableauPile()
            {
                cards = addingTableau
            };

            // Act
            testRules.MoveToTableau(card4, discard, addingTableauPile);

            // Assert
            Assert.True(addingTableauPile.ValidatePile());
            Assert.Equal(3, addingTableauPile.Count());
            Assert.Equal(3, discard.Count());
        }

        [Fact]
        public void FailToMoveNonLastCardFromDiscardToOccupiedTableau()
        {
            // Arrange
            var testRules = new SolitaireRules();

            var card1 = new Card { CardNumber = Number.Four, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Number.Nine, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire };
            var card3 = new Card { CardNumber = Number.Ten, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire };
            var card4 = new Card { CardNumber = Number.Two, CardSuit = Suit.Clubs, FacingUp = true, Game = GameType.Solitaire };
            List<Card> cards =
            [
                card1,
                card2,
                card3,
                card4
            ];
            var discard = new TableauPile()
            {
                cards = cards
            };

            var card5 = new Card { CardNumber = Number.Four, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            var card6 = new Card { CardNumber = Number.Three, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire };
            List<Card> addingTableau =
            [
                card5,
                card6
            ];
            var addingTableauPile = new TableauPile()
            {
                cards = addingTableau
            };

            // Act
            testRules.MoveToTableau(card3, discard, addingTableauPile);

            // Assert
            Assert.True(addingTableauPile.ValidatePile());
            Assert.Equal(2, addingTableauPile.Count());
            Assert.Equal(4, discard.Count());
        }

        [Fact]
        public void FailToMoveCardToOccupiedTableau() // Attempts to move a card in a tableau with a difference of 2
        {
            // Arrange
            var testRules = new SolitaireRules();

            var selectedCard = new Card { CardNumber = Number.Two, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            var extraCard = new Card { CardNumber = Number.Ace, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire };
            List<Card> chosenTableau =
            [
                selectedCard,
                extraCard
            ];
            var chosenTableauPile = new TableauPile()
            {
                cards = chosenTableau
            };

            var card1 = new Card { CardNumber = Number.Ten, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Number.Nine, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire };
            var card3 = new Card { CardNumber = Number.Eight, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            List<Card> addingTableau =
            [
                card1,
                card2,
                card3
            ];
            var addingTableauPile = new TableauPile()
            {
                cards = addingTableau
            };

            // Act
            testRules.MoveToTableau(selectedCard, chosenTableauPile, addingTableauPile);

            // Assert
            Assert.Equal(2, chosenTableauPile.Count());
            Assert.Equal(3, addingTableauPile.Count());
        }

        [Fact]
        public void RefillStockWithDiscard()
        {
            // Arrange
            var testRules = new SolitaireRules();

            List<Card> stockCards = new List<Card>();
            testRules.Stock.cards = stockCards;

            var card1 = new Card { CardNumber = Number.Four, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            var card2 = new Card { CardNumber = Number.Ten, CardSuit = Suit.Hearts, FacingUp = true, Game = GameType.Solitaire };
            var card3 = new Card { CardNumber = Number.King, CardSuit = Suit.Diamonds, FacingUp = true, Game = GameType.Solitaire };
            List<Card> discardCards =
            [
                card1,
                card2,
                card3
            ];
            testRules.Discard.cards = discardCards;

            // Act
            testRules.DrawFromStockpile();

            // Assert
            Assert.True(testRules.Discard.IsEmpty());
            Assert.Equal(3, testRules.Stock.Count());
        }

        [Fact]
        public void MoveCardFromStockToDiscard()
        {
            // Arrange
            var testRules = new SolitaireRules();

            var card1 = new Card { CardNumber = Number.Jack, CardSuit = Suit.Clubs, FacingUp = false, Game = GameType.Solitaire };
            List<Card> stockCards =
            [
                card1
            ];
            testRules.Stock.cards = stockCards;

            var card2 = new Card { CardNumber = Number.Four, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            var card3 = new Card { CardNumber = Number.Ace, CardSuit = Suit.Spades, FacingUp = true, Game = GameType.Solitaire };
            List<Card> discardCards =
            [
                card2,
                card3
            ];
            testRules.Discard.cards = discardCards;

            // Act
            testRules.DrawFromStockpile();

            // Assert
            Assert.True(testRules.Stock.IsEmpty());
            Assert.Equal(3, testRules.Discard.Count());
            Assert.True(testRules.Discard.LastCard()!.FacingUp);
        }

        [Fact]
        public void EmptyStockAndDiscardRemainEmpty()
        {
            // Arrange
            var testRules = new SolitaireRules();

            List<Card> stockCards = new List<Card>();
            testRules.Stock.cards = stockCards;

            List<Card> discardCards = new List<Card>();
            testRules.Discard.cards = discardCards;

            // Act
            testRules.DrawFromStockpile();

            // Assert
            Assert.True(testRules.Stock.IsEmpty());
            Assert.True(testRules.Discard.IsEmpty());
        }
    }
}
