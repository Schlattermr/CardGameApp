using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Backend.Models.DTOs;
using Backend.Repository;

namespace Backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly string _connectionString = DatabaseUtilities.CreateConnectionString();

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Register request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            Console.WriteLine("[WARNING] Registration failed: Username or password is empty.");
            return BadRequest("Username and password are required.");
        }

        Console.WriteLine($"[INFO] Registration request received for username: {request.Username}");
        try
        {
            var existingUserId = await UserRepository.GetUserIdAsync(request.Username, _connectionString);
            if (existingUserId.HasValue)
            {
                Console.WriteLine($"[WARNING] Registration failed: Username '{request.Username}' already exists.");
                return BadRequest("Username already exists.");
            }

            if (!Regex.IsMatch(request.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                Console.WriteLine($"[WARNING] Registration failed: Password for username '{request.Username}' does not meet requirements.");
                return BadRequest("Password must be at least 8 characters long and contain one uppercase and one lowercase letter, one number, and one special character (@$!%*?&).");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            await UserRepository.AddNewUserAsync(request.Username, hashedPassword, _connectionString);

            Console.WriteLine($"[INFO] User '{request.Username}' registered successfully.");
            return Ok("User registered successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Error during registration for username '{request.Username}': {ex.Message}");
            return StatusCode(500, "An internal error occurred during registration.");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            Console.WriteLine("[WARNING] Login failed: Username or password is empty.");
            return BadRequest("Username and password are required.");
        }

        Console.WriteLine($"[INFO] Login request received for username: {request.Username}");
        try
        {
            var userId = await UserRepository.GetUserIdAsync(request.Username, _connectionString);
            if (!userId.HasValue)
            {
                Console.WriteLine($"[WARNING] Login failed: Username '{request.Username}' not found.");
                return Unauthorized("Invalid username or password.");
            }

            var passwordHash = await UserRepository.GetPasswordHashAsync((int)userId, _connectionString);
            if (string.IsNullOrEmpty(passwordHash))
            {
                Console.WriteLine($"[ERROR] Password hash for user '{request.Username}' is null or empty.");
                return Unauthorized("Invalid username or password.");
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, passwordHash);
            if (!isPasswordValid)
            {
                Console.WriteLine($"[WARNING] Login failed: Incorrect password for username '{request.Username}'.");
                return Unauthorized("Invalid username or password.");
            }

            var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            if (string.IsNullOrEmpty(jwtSecretKey))
            {
                Console.WriteLine("[ERROR] JWT_SECRET_KEY environment variable is not set.");
                return StatusCode(500, "Server configuration error. Please contact support.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "procrastination-pastimes",
                audience: "procrastination-pastimes",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            Console.WriteLine($"[INFO] User '{request.Username}' logged in successfully.");
            return Ok(new { token = tokenString, username = request.Username });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Error during login for username '{request.Username}': {ex.Message}");
            return StatusCode(500, "An internal error occurred during login.");
        }
    }
}
