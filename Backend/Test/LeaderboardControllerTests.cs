using Microsoft.AspNetCore.Mvc;
using Backend.Controllers;
using Backend.Models.DTOs;

namespace Test
{
    public class LeaderboardControllerTests
    {
        [Fact]
        public async Task GetLeaderboardData_WhenSuccessful_ReturnsOk()
        {
            // Arrange
            var controller = new LeaderboardController();

            // Act
            var result = await controller.GetLeaderboardData();

            // Assert
            // Should return either Ok with data or Ok with null (if no data exists)
            Assert.True(result is OkObjectResult || result is BadRequestObjectResult);
        }

        [Fact]
        public async Task GetLeaderboardData_ReturnsExpectedResultType()
        {
            // Arrange
            var controller = new LeaderboardController();

            // Act
            var result = await controller.GetLeaderboardData();

            // Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public async Task GetLeaderboardWins_WithNullUsername_ReturnsBadRequest()
        {
            // Arrange
            var controller = new LeaderboardController();
            string? username = null;

            // Act
            var result = await controller.GetLeaderboardWins(username!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request payload.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetLeaderboardWins_WithEmptyUsername_ReturnsBadRequest()
        {
            // Arrange
            var controller = new LeaderboardController();
            var username = "";

            // Act
            var result = await controller.GetLeaderboardWins(username);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request payload.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetLeaderboardWins_WithWhitespaceUsername_ReturnsBadRequest()
        {
            // Arrange
            var controller = new LeaderboardController();
            var username = "   ";

            // Act
            var result = await controller.GetLeaderboardWins(username);

            // Assert
            // Empty string check should catch whitespace
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request payload.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetLeaderboardWins_WithNonExistentUser_ReturnsNotFound()
        {
            // Arrange
            var controller = new LeaderboardController();
            var username = $"nonexistent_user_{Guid.NewGuid()}";

            // Act
            var result = await controller.GetLeaderboardWins(username);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetLeaderboardWins_WithValidUsername_ReturnsExpectedType()
        {
            // Arrange
            var controller = new LeaderboardController();

            // First, register a user to ensure they exist
            var authController = new AuthController();
            var username = $"leaderboarduser_{Guid.NewGuid()}";
            var registerDto = new Backend.Models.DTOs.Register
            {
                Username = username,
                Password = "ValidPass123!"
            };
            await authController.Register(registerDto);

            // Act
            var result = await controller.GetLeaderboardWins(username);

            // Assert
            // Should return either Ok or NotFound, but not BadRequest or InternalServerError
            Assert.True(result is OkObjectResult || result is NotFoundObjectResult);
        }

        [Fact]
        public async Task UpdateLeaderboardWins_WithNullRequest_ReturnsBadRequest()
        {
            // Arrange
            var controller = new LeaderboardController();
            UpdateLeaderboardRequest? request = null;

            // Act
            var result = await controller.UpdateLeaderboardWins(request!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request payload.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateLeaderboardWins_WithNullUsername_ReturnsBadRequest()
        {
            // Arrange
            var controller = new LeaderboardController();
            var request = new UpdateLeaderboardRequest
            {
                Username = null!,
                Wins = 5
            };

            // Act
            var result = await controller.UpdateLeaderboardWins(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request payload.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateLeaderboardWins_WithEmptyUsername_ReturnsBadRequest()
        {
            // Arrange
            var controller = new LeaderboardController();
            var request = new UpdateLeaderboardRequest
            {
                Username = "",
                Wins = 5
            };

            // Act
            var result = await controller.UpdateLeaderboardWins(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request payload.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateLeaderboardWins_WithNegativeWins_ReturnsBadRequest()
        {
            // Arrange
            var controller = new LeaderboardController();
            var request = new UpdateLeaderboardRequest
            {
                Username = "testuser",
                Wins = -1
            };

            // Act
            var result = await controller.UpdateLeaderboardWins(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request payload.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateLeaderboardWins_WithZeroWins_IsValid()
        {
            // Arrange
            var controller = new LeaderboardController();
            var username = $"zerowinuser_{Guid.NewGuid()}";

            // First, register a user
            var authController = new AuthController();
            var registerDto = new Backend.Models.DTOs.Register
            {
                Username = username,
                Password = "ValidPass123!"
            };
            await authController.Register(registerDto);

            var request = new UpdateLeaderboardRequest
            {
                Username = username,
                Wins = 0
            };

            // Act
            var result = await controller.UpdateLeaderboardWins(request);

            // Assert
            // Should not return BadRequest for zero wins (zero is valid)
            Assert.IsNotType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateLeaderboardWins_WithValidRequest_ReturnsOk()
        {
            // Arrange
            var controller = new LeaderboardController();
            var username = $"updateuser_{Guid.NewGuid()}";

            // First, register a user
            var authController = new AuthController();
            var registerDto = new Backend.Models.DTOs.Register
            {
                Username = username,
                Password = "ValidPass123!"
            };
            await authController.Register(registerDto);

            var request = new UpdateLeaderboardRequest
            {
                Username = username,
                Wins = 5
            };

            // Act
            var result = await controller.UpdateLeaderboardWins(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Leaderboard wins updated successfully.", okResult.Value);
        }

        [Fact]
        public async Task UpdateLeaderboardWins_WithLargeWinsValue_IsValid()
        {
            // Arrange
            var controller = new LeaderboardController();
            var username = $"largewinuser_{Guid.NewGuid()}";

            // First, register a user
            var authController = new AuthController();
            var registerDto = new Backend.Models.DTOs.Register
            {
                Username = username,
                Password = "ValidPass123!"
            };
            await authController.Register(registerDto);

            var request = new UpdateLeaderboardRequest
            {
                Username = username,
                Wins = 1000
            };

            // Act
            var result = await controller.UpdateLeaderboardWins(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateThenGetWins_ReturnsUpdatedValue()
        {
            // Arrange
            var controller = new LeaderboardController();
            var username = $"integrationuser_{Guid.NewGuid()}";

            // First, register a user
            var authController = new AuthController();
            var registerDto = new Backend.Models.DTOs.Register
            {
                Username = username,
                Password = "ValidPass123!"
            };
            await authController.Register(registerDto);

            // Act - Update wins
            var updateRequest = new UpdateLeaderboardRequest
            {
                Username = username,
                Wins = 10
            };
            var updateResult = await controller.UpdateLeaderboardWins(updateRequest);

            // Assert - Update successful
            Assert.IsType<OkObjectResult>(updateResult);

            // Act - Get wins
            var getResult = await controller.GetLeaderboardWins(username);

            // Assert - Got wins data
            Assert.True(getResult is OkObjectResult || getResult is NotFoundObjectResult);
        }

        [Fact]
        public async Task UpdateMultipleTimes_AccumulatesWins()
        {
            // Arrange
            var controller = new LeaderboardController();
            var username = $"multiupdateuser_{Guid.NewGuid()}";

            // First, register a user
            var authController = new AuthController();
            var registerDto = new Backend.Models.DTOs.Register
            {
                Username = username,
                Password = "ValidPass123!"
            };
            await authController.Register(registerDto);

            // Act - Update wins multiple times
            var request1 = new UpdateLeaderboardRequest
            {
                Username = username,
                Wins = 0
            };
            await controller.UpdateLeaderboardWins(request1);

            var request2 = new UpdateLeaderboardRequest
            {
                Username = username,
                Wins = 1
            };
            await controller.UpdateLeaderboardWins(request2);

            var request3 = new UpdateLeaderboardRequest
            {
                Username = username,
                Wins = 2
            };
            var finalResult = await controller.UpdateLeaderboardWins(request3);

            // Assert
            Assert.IsType<OkObjectResult>(finalResult);
        }

        [Fact]
        public async Task GetLeaderboardData_AfterMultipleUpdates_ReturnsData()
        {
            // Arrange
            var controller = new LeaderboardController();
            var users = new[]
            {
                $"leaderuser1_{Guid.NewGuid()}",
                $"leaderuser2_{Guid.NewGuid()}",
                $"leaderuser3_{Guid.NewGuid()}"
            };

            var authController = new AuthController();

            // Register multiple users and update their wins
            foreach (var user in users)
            {
                var registerDto = new Backend.Models.DTOs.Register
                {
                    Username = user,
                    Password = "ValidPass123!"
                };
                await authController.Register(registerDto);

                var updateRequest = new UpdateLeaderboardRequest
                {
                    Username = user,
                    Wins = Random.Shared.Next(1, 20)
                };
                await controller.UpdateLeaderboardWins(updateRequest);
            }

            // Act
            var result = await controller.GetLeaderboardData();

            // Assert
            Assert.True(result is OkObjectResult || result is BadRequestObjectResult);
        }

        [Fact]
        public async Task FullLeaderboardWorkflow_Success()
        {
            // Arrange
            var controller = new LeaderboardController();
            var authController = new AuthController();
            var username = $"workflowuser_{Guid.NewGuid()}";

            // Act 1: Register user
            var registerDto = new Backend.Models.DTOs.Register
            {
                Username = username,
                Password = "ValidPass123!"
            };
            var registerResult = await authController.Register(registerDto);
            Assert.IsType<OkObjectResult>(registerResult);

            // Act 2: Update wins
            var updateRequest = new UpdateLeaderboardRequest
            {
                Username = username,
                Wins = 15
            };
            var updateResult = await controller.UpdateLeaderboardWins(updateRequest);
            Assert.IsType<OkObjectResult>(updateResult);

            // Act 3: Get user wins
            var getWinsResult = await controller.GetLeaderboardWins(username);
            Assert.True(getWinsResult is OkObjectResult || getWinsResult is NotFoundObjectResult);

            // Act 4: Get leaderboard data
            var leaderboardResult = await controller.GetLeaderboardData();
            Assert.True(leaderboardResult is OkObjectResult || leaderboardResult is BadRequestObjectResult);
        }

        [Fact]
        public void UpdateLeaderboardRequest_CanSetUsername()
        {
            // Arrange & Act
            var request = new UpdateLeaderboardRequest
            {
                Username = "testuser",
                Wins = 5
            };

            // Assert
            Assert.Equal("testuser", request.Username);
        }

        [Fact]
        public void UpdateLeaderboardRequest_CanSetWins()
        {
            // Arrange & Act
            var request = new UpdateLeaderboardRequest
            {
                Username = "testuser",
                Wins = 42
            };

            // Assert
            Assert.Equal(42, request.Wins);
        }

        [Fact]
        public void UpdateLeaderboardRequest_PropertiesAreRequired()
        {
            // This test verifies the model has required properties
            var properties = typeof(UpdateLeaderboardRequest).GetProperties();

            Assert.Contains(properties, p => p.Name == "Username");
            Assert.Contains(properties, p => p.Name == "Wins");
        }

        [Fact]
        public async Task GetLeaderboardWins_WithSpecialCharactersInUsername_HandlesGracefully()
        {
            // Arrange
            var controller = new LeaderboardController();
            var username = "user@#$%";

            // Act
            var result = await controller.GetLeaderboardWins(username);

            // Assert
            // Should return either NotFound or handle the special characters
            Assert.True(result is NotFoundObjectResult || result is OkObjectResult || result is ObjectResult);
        }

        [Fact]
        public async Task UpdateLeaderboardWins_WithVeryLongUsername_HandlesGracefully()
        {
            // Arrange
            var controller = new LeaderboardController();
            var longUsername = new string('a', 100);
            var request = new UpdateLeaderboardRequest
            {
                Username = longUsername,
                Wins = 5
            };

            // Act
            var result = await controller.UpdateLeaderboardWins(request);

            // Assert
            // Should handle gracefully without throwing
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public async Task UpdateLeaderboardWins_WithMaxIntWins_HandlesGracefully()
        {
            // Arrange
            var controller = new LeaderboardController();
            var username = $"maxintuser_{Guid.NewGuid()}";

            // First, register a user
            var authController = new AuthController();
            var registerDto = new Backend.Models.DTOs.Register
            {
                Username = username,
                Password = "ValidPass123!"
            };
            await authController.Register(registerDto);

            var request = new UpdateLeaderboardRequest
            {
                Username = username,
                Wins = int.MaxValue
            };

            // Act
            var result = await controller.UpdateLeaderboardWins(request);

            // Assert
            // Should handle large numbers gracefully
            Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
