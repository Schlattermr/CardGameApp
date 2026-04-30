using Microsoft.AspNetCore.Mvc;
using Backend.Controllers;
using Backend.Models.DTOs;

namespace Test
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_WithEmptyUsername_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Register { Username = "", Password = "ValidPass123!" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username and password are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Register_WithNullUsername_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Register { Username = null!, Password = "ValidPass123!" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username and password are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Register_WithWhitespaceUsername_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Register { Username = "   ", Password = "ValidPass123!" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username and password are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Register_WithEmptyPassword_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Register { Username = "testuser", Password = "" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username and password are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Register_WithNullPassword_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Register { Username = "testuser", Password = null! };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username and password are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Register_WithWhitespacePassword_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Register { Username = "testuser", Password = "   " };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username and password are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Register_WithPasswordTooShort_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Register { Username = "testuser", Password = "Pass1!" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Password must be at least 8 characters", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task Register_WithPasswordMissingUppercase_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Register { Username = "testuser", Password = "password123!" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Password must be at least 8 characters", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task Register_WithPasswordMissingLowercase_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Register { Username = "testuser", Password = "PASSWORD123!" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Password must be at least 8 characters", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task Register_WithPasswordMissingNumber_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Register { Username = "testuser", Password = "Password!" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Password must be at least 8 characters", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task Register_WithPasswordMissingSpecialCharacter_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Register { Username = "testuser", Password = "Password123" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Password must be at least 8 characters", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task Register_WithValidPasswordContainingAtSymbol_PassesValidation()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Register { Username = $"testuser_{Guid.NewGuid()}", Password = "ValidPass123@" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            // Should not be a BadRequest with password validation error
            if (result is BadRequestObjectResult badRequest)
            {
                Assert.DoesNotContain("Password must be at least 8 characters", badRequest.Value?.ToString() ?? "");
            }
        }

        [Fact]
        public async Task Register_WithValidPasswordContainingDollarSign_PassesValidation()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Register { Username = $"testuser_{Guid.NewGuid()}", Password = "ValidPass123$" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            // Should not be a BadRequest with password validation error
            if (result is BadRequestObjectResult badRequest)
            {
                Assert.DoesNotContain("Password must be at least 8 characters", badRequest.Value?.ToString() ?? "");
            }
        }

        [Fact]
        public async Task Register_WithValidPasswordContainingExclamation_PassesValidation()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Register { Username = $"testuser_{Guid.NewGuid()}", Password = "ValidPass123!" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            // Should not be a BadRequest with password validation error
            if (result is BadRequestObjectResult badRequest)
            {
                Assert.DoesNotContain("Password must be at least 8 characters", badRequest.Value?.ToString() ?? "");
            }
        }

        [Fact]
        public async Task Login_WithEmptyUsername_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Login { Username = "", Password = "ValidPass123!" };

            // Act
            var result = await controller.Login(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username and password are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_WithNullUsername_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Login { Username = null!, Password = "ValidPass123!" };

            // Act
            var result = await controller.Login(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username and password are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_WithWhitespaceUsername_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Login { Username = "   ", Password = "ValidPass123!" };

            // Act
            var result = await controller.Login(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username and password are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_WithEmptyPassword_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Login { Username = "testuser", Password = "" };

            // Act
            var result = await controller.Login(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username and password are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_WithNullPassword_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Login { Username = "testuser", Password = null! };

            // Act
            var result = await controller.Login(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username and password are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_WithWhitespacePassword_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Login { Username = "testuser", Password = "   " };

            // Act
            var result = await controller.Login(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username and password are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_WithNonExistentUser_ReturnsUnauthorized()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Login 
            { 
                Username = $"nonexistent_user_{Guid.NewGuid()}", 
                Password = "ValidPass123!" 
            };

            // Act
            var result = await controller.Login(dto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username or password.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var controller = new AuthController();
            var uniqueUsername = $"testuser_{Guid.NewGuid()}";

            // First register a user
            var registerDto = new Register 
            { 
                Username = uniqueUsername, 
                Password = "CorrectPass123!" 
            };
            await controller.Register(registerDto);

            // Then try to login with wrong password
            var loginDto = new Login 
            { 
                Username = uniqueUsername, 
                Password = "WrongPass123!" 
            };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username or password.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Register_WithValidCredentials_ReturnsOk()
        {
            // Arrange
            var controller = new AuthController();
            var dto = new Register 
            { 
                Username = $"newuser_{Guid.NewGuid()}", 
                Password = "ValidPass123!" 
            };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User registered successfully.", okResult.Value);
        }

        [Fact]
        public async Task Register_WithDuplicateUsername_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();
            var username = $"duplicateuser_{Guid.NewGuid()}";
            var dto = new Register 
            { 
                Username = username, 
                Password = "ValidPass123!" 
            };

            // Act - Register first time
            await controller.Register(dto);

            // Act - Try to register again with same username
            var result = await controller.Register(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username already exists.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var controller = new AuthController();
            var username = $"loginuser_{Guid.NewGuid()}";
            var password = "ValidPass123!";

            // Register user first
            var registerDto = new Register 
            { 
                Username = username, 
                Password = password 
            };
            await controller.Register(registerDto);

            // Login with same credentials
            var loginDto = new Login 
            { 
                Username = username, 
                Password = password 
            };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            // Check if the result contains token and username
            var resultValue = okResult.Value;
            var tokenProperty = resultValue?.GetType().GetProperty("token");
            var usernameProperty = resultValue?.GetType().GetProperty("username");

            Assert.NotNull(tokenProperty);
            Assert.NotNull(usernameProperty);

            var token = tokenProperty?.GetValue(resultValue) as string;
            var returnedUsername = usernameProperty?.GetValue(resultValue) as string;

            Assert.NotNull(token);
            Assert.NotEmpty(token);
            Assert.Equal(username, returnedUsername);
        }

        [Fact]
        public async Task Register_ThenLogin_FullWorkflow_Success()
        {
            // Arrange
            var controller = new AuthController();
            var username = $"workflowuser_{Guid.NewGuid()}";
            var password = "WorkflowPass123!";

            // Act - Register
            var registerDto = new Register 
            { 
                Username = username, 
                Password = password 
            };
            var registerResult = await controller.Register(registerDto);

            // Assert - Registration successful
            var registerOkResult = Assert.IsType<OkObjectResult>(registerResult);
            Assert.Equal("User registered successfully.", registerOkResult.Value);

            // Act - Login
            var loginDto = new Login 
            { 
                Username = username, 
                Password = password 
            };
            var loginResult = await controller.Login(loginDto);

            // Assert - Login successful with token
            var loginOkResult = Assert.IsType<OkObjectResult>(loginResult);
            Assert.NotNull(loginOkResult.Value);
        }
    }
}
