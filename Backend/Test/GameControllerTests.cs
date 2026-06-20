using Microsoft.AspNetCore.Mvc;
using Backend.Controllers;
using Backend.Models.Domain;
using System.Reflection;

namespace Test
{
    public class GameControllerTests : IDisposable
    {
        private bool disposed = false;

        public GameControllerTests()
        {
            // Clear the static LoggedInUsers dictionary before each test
            ClearLoggedInUsers();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Clear the static LoggedInUsers dictionary after each test
                    ClearLoggedInUsers();
                }
                disposed = true;
            }
        }

        private static void ClearLoggedInUsers()
        {
            var loggedInUsersField = typeof(GameController).GetField("LoggedInUsers", 
                BindingFlags.NonPublic | BindingFlags.Static);

            if (loggedInUsersField != null)
            {
                var loggedInUsers = loggedInUsersField.GetValue(null) as System.Collections.Concurrent.ConcurrentDictionary<string, bool>;
                loggedInUsers?.Clear();
            }
        }

        [Fact]
        public void GetLoggedInPlayers_WithNoPlayers_ReturnsEmptyList()
        {
            // Arrange
            var controller = new GameController();

            // Act
            var result = controller.GetLoggedInPlayers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var players = Assert.IsAssignableFrom<List<object>>(okResult.Value);
            Assert.Empty(players);
        }

        [Fact]
        public void GetLoggedInPlayers_WithOnePlayer_ReturnsOnePlayer()
        {
            // Arrange
            var controller = new GameController();
            var username = "player1";
            controller.AddLoggedInPlayer(username);

            // Act
            var result = controller.GetLoggedInPlayers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var players = Assert.IsAssignableFrom<System.Collections.IEnumerable>(okResult.Value);
            Assert.Single(players.Cast<object>());
        }

        [Fact]
        public void GetLoggedInPlayers_WithMultiplePlayers_ReturnsAllPlayers()
        {
            // Arrange
            var controller = new GameController();
            var usernames = new[] { "player1", "player2", "player3" };

            foreach (var username in usernames)
            {
                controller.AddLoggedInPlayer(username);
            }

            // Act
            var result = controller.GetLoggedInPlayers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var players = Assert.IsAssignableFrom<System.Collections.IEnumerable>(okResult.Value);
            Assert.Equal(3, players.Cast<object>().Count());
        }

        [Fact]
        public void AddLoggedInPlayer_WithValidUsername_ReturnsOk()
        {
            // Arrange
            var controller = new GameController();
            var username = "validuser";

            // Act
            var result = controller.AddLoggedInPlayer(username);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains(username, okResult.Value?.ToString());
            Assert.Contains("added to the waiting room", okResult.Value?.ToString());
        }

        [Fact]
        public void AddLoggedInPlayer_WithEmptyUsername_ReturnsBadRequest()
        {
            // Arrange
            var controller = new GameController();
            var username = "";

            // Act
            var result = controller.AddLoggedInPlayer(username);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid username.", badRequestResult.Value);
        }

        [Fact]
        public void AddLoggedInPlayer_WithNullUsername_ReturnsBadRequest()
        {
            // Arrange
            var controller = new GameController();
            string username = null!;

            // Act
            var result = controller.AddLoggedInPlayer(username);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid username.", badRequestResult.Value);
        }

        [Fact]
        public void AddLoggedInPlayer_WithWhitespaceUsername_ReturnsBadRequest()
        {
            // Arrange
            var controller = new GameController();
            var username = "   ";

            // Act
            var result = controller.AddLoggedInPlayer(username);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid username.", badRequestResult.Value);
        }

        [Fact]
        public void AddLoggedInPlayer_WithDuplicateUsername_ReturnsConflict()
        {
            // Arrange
            var controller = new GameController();
            var username = "duplicateuser";

            // Act
            controller.AddLoggedInPlayer(username);
            var result = controller.AddLoggedInPlayer(username);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("User is already logged in.", conflictResult.Value);
        }

        [Fact]
        public void AddLoggedInPlayer_AddMultipleUniqueUsers_AllSucceed()
        {
            // Arrange
            var controller = new GameController();
            var usernames = new[] { "user1", "user2", "user3" };

            // Act & Assert
            foreach (var username in usernames)
            {
                var result = controller.AddLoggedInPlayer(username);
                Assert.IsType<OkObjectResult>(result);
            }

            // Verify all users are in the list
            var playersResult = controller.GetLoggedInPlayers();
            var okResult = Assert.IsType<OkObjectResult>(playersResult);
            var players = Assert.IsAssignableFrom<System.Collections.IEnumerable>(okResult.Value);
            Assert.Equal(3, players.Cast<object>().Count());
        }

        [Fact]
        public void AddLoggedInPlayer_CaseSensitiveUsername_TreatsAsUnique()
        {
            // Arrange
            var controller = new GameController();

            // Act
            var result1 = controller.AddLoggedInPlayer("UserName");
            var result2 = controller.AddLoggedInPlayer("username");

            // Assert
            Assert.IsType<OkObjectResult>(result1);
            Assert.IsType<OkObjectResult>(result2);

            var playersResult = controller.GetLoggedInPlayers();
            var okResult = Assert.IsType<OkObjectResult>(playersResult);
            var players = Assert.IsAssignableFrom<System.Collections.IEnumerable>(okResult.Value);
            Assert.Equal(2, players.Cast<object>().Count());
        }

        [Fact]
        public void InitializeGame_WithValidPlayerList_ReturnsOk()
        {
            // Arrange
            var controller = new GameController();
            var playerNames = new List<string> { "player1", "player2" };

            // Act
            var result = controller.InitializeGame(playerNames);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var distributedDeck = Assert.IsType<Dictionary<string, List<Card>>>(okResult.Value);
            Assert.NotNull(distributedDeck);
        }

        [Fact]
        public void InitializeGame_WithNullPlayerList_ReturnsBadRequest()
        {
            // Arrange
            var controller = new GameController();
            List<string>? playerNames = null;

            // Act
            var result = controller.InitializeGame(playerNames!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Player names are required to initialize the game.", badRequestResult.Value);
        }

        [Fact]
        public void InitializeGame_WithEmptyPlayerList_ReturnsBadRequest()
        {
            // Arrange
            var controller = new GameController();
            var playerNames = new List<string>();

            // Act
            var result = controller.InitializeGame(playerNames);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Player names are required to initialize the game.", badRequestResult.Value);
        }

        [Fact]
        public void InitializeGame_WithTwoPlayers_DistributesCardsEvenly()
        {
            // Arrange
            var controller = new GameController();
            var playerNames = new List<string> { "player1", "player2" };

            // Act
            var result = controller.InitializeGame(playerNames);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var distributedDeck = Assert.IsType<Dictionary<string, List<Card>>>(okResult.Value);

            Assert.Equal(2, distributedDeck.Count);
            Assert.True(distributedDeck.ContainsKey("player1"));
            Assert.True(distributedDeck.ContainsKey("player2"));

            // Each player should have 26 cards (52 / 2)
            Assert.Equal(26, distributedDeck["player1"].Count);
            Assert.Equal(26, distributedDeck["player2"].Count);
        }

        [Fact]
        public void InitializeGame_WithFourPlayers_DistributesCardsEvenly()
        {
            // Arrange
            var controller = new GameController();
            var playerNames = new List<string> { "player1", "player2", "player3", "player4" };

            // Act
            var result = controller.InitializeGame(playerNames);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var distributedDeck = Assert.IsType<Dictionary<string, List<Card>>>(okResult.Value);

            Assert.Equal(4, distributedDeck.Count);

            // Each player should have 13 cards (52 / 4)
            foreach (var player in playerNames)
            {
                Assert.True(distributedDeck.ContainsKey(player));
                Assert.Equal(13, distributedDeck[player].Count);
            }
        }

        [Fact]
        public void InitializeGame_WithSinglePlayer_ReturnsAllCards()
        {
            // Arrange
            var controller = new GameController();
            var playerNames = new List<string> { "player1" };

            // Act
            var result = controller.InitializeGame(playerNames);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var distributedDeck = Assert.IsType<Dictionary<string, List<Card>>>(okResult.Value);

            Assert.Single(distributedDeck);
            Assert.Equal(52, distributedDeck["player1"].Count);
        }

        [Fact]
        public void InitializeGame_CalledTwice_ReturnsDifferentDecks()
        {
            // Arrange
            var controller = new GameController();
            var playerNames = new List<string> { "player1" };

            // Act
            var result1 = controller.InitializeGame(playerNames);
            var result2 = controller.InitializeGame(playerNames);

            // Assert
            var okResult1 = Assert.IsType<OkObjectResult>(result1);
            var okResult2 = Assert.IsType<OkObjectResult>(result2);

            var deck1 = Assert.IsType<Dictionary<string, List<Card>>>(okResult1.Value);
            var deck2 = Assert.IsType<Dictionary<string, List<Card>>>(okResult2.Value);

            // Decks should be shuffled differently (with very high probability)
            var cards1 = deck1["player1"];
            var cards2 = deck2["player1"];

            // Check if at least one card is in a different position
            var isDifferent = false;
            for (int i = 0; i < cards1.Count; i++)
            {
                if (cards1[i].CardNumber != cards2[i].CardNumber || 
                    cards1[i].CardSuit != cards2[i].CardSuit)
                {
                    isDifferent = true;
                    break;
                }
            }

            Assert.True(isDifferent, "Two shuffled decks should be different");
        }

        [Fact]
        public void ResetGame_WithValidPlayerList_ReturnsOk()
        {
            // Arrange
            var controller = new GameController();
            var playerNames = new List<string> { "player1", "player2" };

            // Act
            var result = controller.ResetGame(playerNames);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var distributedDeck = Assert.IsType<Dictionary<string, List<Card>>>(okResult.Value);
            Assert.NotNull(distributedDeck);
        }

        [Fact]
        public void ResetGame_WithNullPlayerList_ReturnsBadRequest()
        {
            // Arrange
            var controller = new GameController();
            List<string>? playerNames = null;

            // Act
            var result = controller.ResetGame(playerNames!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Player names are required to reset the game.", badRequestResult.Value);
        }

        [Fact]
        public void ResetGame_WithEmptyPlayerList_ReturnsBadRequest()
        {
            // Arrange
            var controller = new GameController();
            var playerNames = new List<string>();

            // Act
            var result = controller.ResetGame(playerNames);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Player names are required to reset the game.", badRequestResult.Value);
        }

        [Fact]
        public void ResetGame_WithTwoPlayers_DistributesCardsEvenly()
        {
            // Arrange
            var controller = new GameController();
            var playerNames = new List<string> { "player1", "player2" };

            // Act
            var result = controller.ResetGame(playerNames);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var distributedDeck = Assert.IsType<Dictionary<string, List<Card>>>(okResult.Value);

            Assert.Equal(2, distributedDeck.Count);
            Assert.Equal(26, distributedDeck["player1"].Count);
            Assert.Equal(26, distributedDeck["player2"].Count);
        }

        [Fact]
        public void ResetGame_AfterInitialize_ReturnsDifferentDeck()
        {
            // Arrange
            var controller = new GameController();
            var playerNames = new List<string> { "player1" };

            // Act
            var initResult = controller.InitializeGame(playerNames);
            var resetResult = controller.ResetGame(playerNames);

            // Assert
            var initOkResult = Assert.IsType<OkObjectResult>(initResult);
            var resetOkResult = Assert.IsType<OkObjectResult>(resetResult);

            var initDeck = Assert.IsType<Dictionary<string, List<Card>>>(initOkResult.Value);
            var resetDeck = Assert.IsType<Dictionary<string, List<Card>>>(resetOkResult.Value);

            // Decks should be shuffled differently (with very high probability)
            var initCards = initDeck["player1"];
            var resetCards = resetDeck["player1"];

            var isDifferent = false;
            for (int i = 0; i < initCards.Count; i++)
            {
                if (initCards[i].CardNumber != resetCards[i].CardNumber || 
                    initCards[i].CardSuit != resetCards[i].CardSuit)
                {
                    isDifferent = true;
                    break;
                }
            }

            Assert.True(isDifferent, "Reset should create a newly shuffled deck");
        }

        [Fact]
        public void FullGameFlow_AddPlayersInitializeAndReset_Success()
        {
            // Arrange
            var controller = new GameController();
            var player1 = "player1";
            var player2 = "player2";

            // Act - Add players
            var addResult1 = controller.AddLoggedInPlayer(player1);
            var addResult2 = controller.AddLoggedInPlayer(player2);

            // Assert - Players added successfully
            Assert.IsType<OkObjectResult>(addResult1);
            Assert.IsType<OkObjectResult>(addResult2);

            // Act - Get players
            var playersResult = controller.GetLoggedInPlayers();
            var playersOkResult = Assert.IsType<OkObjectResult>(playersResult);
            var players = Assert.IsAssignableFrom<System.Collections.IEnumerable>(playersOkResult.Value);

            // Assert - Both players in the list
            Assert.Equal(2, players.Cast<object>().Count());

            // Act - Initialize game
            var initResult = controller.InitializeGame(new List<string> { player1, player2 });
            var initOkResult = Assert.IsType<OkObjectResult>(initResult);
            var initDeck = Assert.IsType<Dictionary<string, List<Card>>>(initOkResult.Value);

            // Assert - Game initialized with cards distributed
            Assert.Equal(2, initDeck.Count);
            Assert.Equal(26, initDeck[player1].Count);
            Assert.Equal(26, initDeck[player2].Count);

            // Act - Reset game
            var resetResult = controller.ResetGame(new List<string> { player1, player2 });
            var resetOkResult = Assert.IsType<OkObjectResult>(resetResult);
            var resetDeck = Assert.IsType<Dictionary<string, List<Card>>>(resetOkResult.Value);

            // Assert - Game reset with new deck
            Assert.Equal(2, resetDeck.Count);
            Assert.Equal(26, resetDeck[player1].Count);
            Assert.Equal(26, resetDeck[player2].Count);
        }

        [Fact]
        public void AddPlayer_ThenGetPlayers_ReturnsAddedPlayer()
        {
            // Arrange
            var controller = new GameController();
            var username = "testplayer";

            // Act
            controller.AddLoggedInPlayer(username);
            var result = controller.GetLoggedInPlayers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var players = okResult.Value as System.Collections.IEnumerable;
            Assert.NotNull(players);

            var playerList = players.Cast<object>().ToList();
            Assert.Single(playerList);
        }
    }
}
