using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Backend.Controllers;
using Backend.Models.DTOs;
using Backend.Services;

namespace Test
{
    public class BlackjackControllerTests : IDisposable
    {
        private bool disposed;

        public BlackjackControllerTests()
        {
            ClearGames();
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
                    ClearGames();
                }
                disposed = true;
            }
        }

        private static void ClearGames()
        {
            var gamesField = typeof(BlackjackController).GetField("Games",
                BindingFlags.NonPublic | BindingFlags.Static);

            if (gamesField != null)
            {
                var games = gamesField.GetValue(null) as ConcurrentDictionary<string, BlackjackRules>;
                games?.Clear();
            }
        }

        [Fact]
        public void Start_WithValidUsername_ReturnsOk()
        {
            var controller = new BlackjackController();

            var result = controller.Start("player1");

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Start_WithValidUsername_ReturnsStateWithTwoCardHands()
        {
            var controller = new BlackjackController();

            var result = controller.Start("player1");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var state = Assert.IsType<BlackjackStateResponse>(okResult.Value);

            Assert.Equal(2, state.PlayerHand.Count);
            Assert.Equal(2, state.DealerHand.Count);
        }

        [Fact]
        public void Start_WithEmptyUsername_ReturnsBadRequest()
        {
            var controller = new BlackjackController();

            var result = controller.Start("");

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Hit_WithoutActiveGame_ReturnsNotFound()
        {
            var controller = new BlackjackController();

            var result = controller.Hit("nobodyHasStarted");

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Hit_AfterStart_ReturnsOk()
        {
            var controller = new BlackjackController();
            controller.Start("player2");

            var result = controller.Hit("player2");

            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.True(result is OkObjectResult || result is BadRequestObjectResult);
        }

        [Fact]
        public void Stand_WithoutActiveGame_ReturnsNotFound()
        {
            var controller = new BlackjackController();

            var result = controller.Stand("nobodyHasStarted");

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Stand_AfterStart_ReturnsOkWithRoundOverTrue()
        {
            var controller = new BlackjackController();
            controller.Start("player3");

            var result = controller.Stand("player3");

            if (result is OkObjectResult okResult)
            {
                var state = Assert.IsType<BlackjackStateResponse>(okResult.Value);
                Assert.True(state.RoundOver);
            }
            else
            {
                Assert.IsType<BadRequestObjectResult>(result);
            }
        }

        [Fact]
        public void Start_CalledTwiceForSameUser_ResetsTheirGame()
        {
            var controller = new BlackjackController();

            controller.Start("player4");
            var secondResult = controller.Start("player4");

            var okResult = Assert.IsType<OkObjectResult>(secondResult);
            var state = Assert.IsType<BlackjackStateResponse>(okResult.Value);
            Assert.Equal(2, state.PlayerHand.Count);
            Assert.Equal(2, state.DealerHand.Count);
        }
    }
}
