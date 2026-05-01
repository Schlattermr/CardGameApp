using Microsoft.AspNetCore.Mvc;
using Backend.Controllers;
using Backend.Models.Domain;
using Backend.Models.Enums;
using Backend.Models.DTOs;

namespace Test
{
    public class SolitaireControllerTests
    {
        [Fact]
        public void MoveCard_WithValidMove_ReturnsOk()
        {
            // Arrange
            var controller = new SolitaireController();
            var card = new Card
            {
                CardNumber = Number.King,
                CardSuit = Suit.Hearts,
                FacingUp = true,
                Game = GameType.Solitaire
            };

            var sourcePile = new Pile
            {
                Cards = new List<Card> { card }
            };

            var targetPile = new TableauPile
            {
                Cards = new List<Card>()
            };

            var request = new MoveCardRequest
            {
                SelectedCard = card,
                SourcePile = sourcePile,
                TargetPile = targetPile
            };

            // Act
            var result = controller.MoveCard(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void MoveCard_WithValidMove_ReturnsCorrectResponseType()
        {
            // Arrange
            var controller = new SolitaireController();
            var card = new Card
            {
                CardNumber = Number.King,
                CardSuit = Suit.Hearts,
                FacingUp = true,
                Game = GameType.Solitaire
            };

            var sourcePile = new Pile
            {
                Cards = new List<Card> { card }
            };

            var targetPile = new TableauPile
            {
                Cards = new List<Card>()
            };

            var request = new MoveCardRequest
            {
                SelectedCard = card,
                SourcePile = sourcePile,
                TargetPile = targetPile
            };

            // Act
            var result = controller.MoveCard(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<MoveCardResponse>(okResult.Value);
        }

        [Fact]
        public void MoveCard_WithValidMove_ResponseContainsAllPiles()
        {
            // Arrange
            var controller = new SolitaireController();
            var card = new Card
            {
                CardNumber = Number.King,
                CardSuit = Suit.Hearts,
                FacingUp = true,
                Game = GameType.Solitaire
            };

            var sourcePile = new Pile
            {
                Cards = new List<Card> { card }
            };

            var targetPile = new TableauPile
            {
                Cards = new List<Card>()
            };

            var request = new MoveCardRequest
            {
                SelectedCard = card,
                SourcePile = sourcePile,
                TargetPile = targetPile
            };

            // Act
            var result = controller.MoveCard(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MoveCardResponse>(okResult.Value);

            Assert.NotNull(response.Tableau1);
            Assert.NotNull(response.Tableau2);
            Assert.NotNull(response.Tableau3);
            Assert.NotNull(response.Tableau4);
            Assert.NotNull(response.Tableau5);
            Assert.NotNull(response.Tableau6);
            Assert.NotNull(response.Tableau7);
            Assert.NotNull(response.FoundationClubs);
            Assert.NotNull(response.FoundationDiamonds);
            Assert.NotNull(response.FoundationHearts);
            Assert.NotNull(response.FoundationSpades);
            Assert.NotNull(response.Stock);
            Assert.NotNull(response.Discard);
        }

        [Fact]
        public void MoveCard_WithInvalidMove_ReturnsBadRequest()
        {
            // Arrange
            var controller = new SolitaireController();

            // Create an invalid move scenario - trying to place a red card on a red card
            var redCard = new Card
            {
                CardNumber = Number.Five,
                CardSuit = Suit.Hearts,
                FacingUp = true,
                Game = GameType.Solitaire
            };

            var targetCard = new Card
            {
                CardNumber = Number.Six,
                CardSuit = Suit.Diamonds,
                FacingUp = true,
                Game = GameType.Solitaire
            };

            var sourcePile = new Pile
            {
                Cards = new List<Card> { redCard }
            };

            var targetPile = new TableauPile
            {
                Cards = new List<Card> { targetCard }
            };

            var request = new MoveCardRequest
            {
                SelectedCard = redCard,
                SourcePile = sourcePile,
                TargetPile = targetPile
            };

            // Act
            var result = controller.MoveCard(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void MoveCard_WithInvalidMove_ReturnsErrorMessage()
        {
            // Arrange
            var controller = new SolitaireController();

            var redCard = new Card
            {
                CardNumber = Number.Five,
                CardSuit = Suit.Hearts,
                FacingUp = true,
                Game = GameType.Solitaire
            };

            var targetCard = new Card
            {
                CardNumber = Number.Six,
                CardSuit = Suit.Diamonds,
                FacingUp = true,
                Game = GameType.Solitaire
            };

            var sourcePile = new Pile
            {
                Cards = new List<Card> { redCard }
            };

            var targetPile = new TableauPile
            {
                Cards = new List<Card> { targetCard }
            };

            var request = new MoveCardRequest
            {
                SelectedCard = redCard,
                SourcePile = sourcePile,
                TargetPile = targetPile
            };

            // Act
            var result = controller.MoveCard(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
            Assert.IsType<string>(badRequestResult.Value);
            Assert.StartsWith("Failed to move card:", badRequestResult.Value.ToString());
        }

        [Fact]
        public void MoveCard_WithValidAlternatingColors_ReturnsOk()
        {
            // Arrange
            var controller = new SolitaireController();

            // Black card on red card - should be valid
            var blackCard = new Card
            {
                CardNumber = Number.Five,
                CardSuit = Suit.Spades,
                FacingUp = true,
                Game = GameType.Solitaire
            };

            var redCard = new Card
            {
                CardNumber = Number.Six,
                CardSuit = Suit.Hearts,
                FacingUp = true,
                Game = GameType.Solitaire
            };

            var sourcePile = new Pile
            {
                Cards = new List<Card> { blackCard }
            };

            var targetPile = new TableauPile
            {
                Cards = new List<Card> { redCard }
            };

            var request = new MoveCardRequest
            {
                SelectedCard = blackCard,
                SourcePile = sourcePile,
                TargetPile = targetPile
            };

            // Act
            var result = controller.MoveCard(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void MoveCard_ReturnsIActionResult()
        {
            // Arrange
            var controller = new SolitaireController();
            var card = new Card
            {
                CardNumber = Number.King,
                CardSuit = Suit.Hearts,
                FacingUp = true,
                Game = GameType.Solitaire
            };

            var sourcePile = new Pile
            {
                Cards = new List<Card> { card }
            };

            var targetPile = new TableauPile
            {
                Cards = new List<Card>()
            };

            var request = new MoveCardRequest
            {
                SelectedCard = card,
                SourcePile = sourcePile,
                TargetPile = targetPile
            };

            // Act
            var result = controller.MoveCard(request);

            // Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void MoveCard_WithEmptyTargetPile_KingCanMove()
        {
            // Arrange
            var controller = new SolitaireController();
            var king = new Card
            {
                CardNumber = Number.King,
                CardSuit = Suit.Clubs,
                FacingUp = true,
                Game = GameType.Solitaire
            };

            var sourcePile = new Pile
            {
                Cards = new List<Card> { king }
            };

            var targetPile = new TableauPile
            {
                Cards = new List<Card>()
            };

            var request = new MoveCardRequest
            {
                SelectedCard = king,
                SourcePile = sourcePile,
                TargetPile = targetPile
            };

            // Act
            var result = controller.MoveCard(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void MoveCard_WithEmptyTargetPile_NonKingCannotMove()
        {
            // Arrange
            var controller = new SolitaireController();
            var queen = new Card
            {
                CardNumber = Number.Queen,
                CardSuit = Suit.Clubs,
                FacingUp = true,
                Game = GameType.Solitaire
            };

            var sourcePile = new Pile
            {
                Cards = new List<Card> { queen }
            };

            var targetPile = new TableauPile
            {
                Cards = new List<Card>()
            };

            var request = new MoveCardRequest
            {
                SelectedCard = queen,
                SourcePile = sourcePile,
                TargetPile = targetPile
            };

            // Act
            var result = controller.MoveCard(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void MoveCard_WithSequentialCards_ReturnsOk()
        {
            // Arrange
            var controller = new SolitaireController();

            var threeOfClubs = new Card
            {
                CardNumber = Number.Three,
                CardSuit = Suit.Clubs,
                FacingUp = true,
                Game = GameType.Solitaire
            };

            var fourOfDiamonds = new Card
            {
                CardNumber = Number.Four,
                CardSuit = Suit.Diamonds,
                FacingUp = true,
                Game = GameType.Solitaire
            };

            var sourcePile = new Pile
            {
                Cards = new List<Card> { threeOfClubs }
            };

            var targetPile = new TableauPile
            {
                Cards = new List<Card> { fourOfDiamonds }
            };

            var request = new MoveCardRequest
            {
                SelectedCard = threeOfClubs,
                SourcePile = sourcePile,
                TargetPile = targetPile
            };

            // Act
            var result = controller.MoveCard(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void MoveCardRequest_CanBeCreatedWithRequiredProperties()
        {
            // Arrange & Act
            var card = new Card
            {
                CardNumber = Number.Ace,
                CardSuit = Suit.Hearts,
                FacingUp = true,
                Game = GameType.Solitaire
            };

            var request = new MoveCardRequest
            {
                SelectedCard = card,
                SourcePile = new Pile(),
                TargetPile = new TableauPile()
            };

            // Assert
            Assert.NotNull(request.SelectedCard);
            Assert.NotNull(request.SourcePile);
            Assert.NotNull(request.TargetPile);
        }

        [Fact]
        public void MoveCardResponse_CanBeCreatedWithAllProperties()
        {
            // Arrange & Act
            var response = new MoveCardResponse
            {
                Tableau1 = new TableauPile(),
                Tableau2 = new TableauPile(),
                Tableau3 = new TableauPile(),
                Tableau4 = new TableauPile(),
                Tableau5 = new TableauPile(),
                Tableau6 = new TableauPile(),
                Tableau7 = new TableauPile(),
                FoundationClubs = new FoundationPile(),
                FoundationDiamonds = new FoundationPile(),
                FoundationHearts = new FoundationPile(),
                FoundationSpades = new FoundationPile(),
                Stock = new Pile(),
                Discard = new Pile()
            };

            // Assert
            Assert.NotNull(response.Tableau1);
            Assert.NotNull(response.Tableau2);
            Assert.NotNull(response.Tableau3);
            Assert.NotNull(response.Tableau4);
            Assert.NotNull(response.Tableau5);
            Assert.NotNull(response.Tableau6);
            Assert.NotNull(response.Tableau7);
            Assert.NotNull(response.FoundationClubs);
            Assert.NotNull(response.FoundationDiamonds);
            Assert.NotNull(response.FoundationHearts);
            Assert.NotNull(response.FoundationSpades);
            Assert.NotNull(response.Stock);
            Assert.NotNull(response.Discard);
        }
    }
}
