using BackendAPI.Data;
using BackendAPI.DTOs;
using BackendAPI.Models;
using Engines;
using Accessors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace BackendAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserAccessor _userAccessor;
    private readonly string? _connectionString;

    public AuthController(UserAccessor userAccessor)
    {
        _userAccessor = userAccessor;
        _connectionString = DatabaseUtilities.CreateConnectionString();
    }

    // Endpoint for user registration
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        Console.WriteLine($"[INFO] Registration request received for username: {dto.Username}");
        try
        {
            // Check if the username already exists
            var existingUserId = await _userAccessor.GetUserIdAsync(dto.Username, _connectionString);
            if (existingUserId.HasValue)
            {
                Console.WriteLine($"[WARNING] Registration failed: Username '{dto.Username}' already exists.");
                return BadRequest("Username already exists.");
            }

            // Hash the password using bcrypt
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Add user to database
            await _userAccessor.AddNewUserAsync(dto.Username, hashedPassword, _connectionString);

            Console.WriteLine($"[INFO] User '{dto.Username}' registered successfully.");
            return Ok("User registered successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Error during registration for username '{dto.Username}': {ex.Message}");
            return StatusCode(500, "An internal error occurred during registration.");
        }
    }

    // Endpoint for user login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        Console.WriteLine($"[INFO] Login request received for username: {dto.Username}");
        try
        {
            // Find the user by username
            var userId = await _userAccessor.GetUserIdAsync(dto.Username, _connectionString);
            if (!userId.HasValue)
            {
                Console.WriteLine($"[WARNING] Login failed: Username '{dto.Username}' not found.");
                return Unauthorized("Invalid username or password.");
            }

            var passwordHash = await _userAccessor.GetPasswordHashAsync((int)userId, _connectionString);
            if (string.IsNullOrEmpty(passwordHash))
            {
                Console.WriteLine($"[ERROR] Password hash for user '{dto.Username}' is null or empty.");
                return Unauthorized("Invalid username or password.");
            }

            // Verify the password using bcrypt
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, passwordHash);
            if (!isPasswordValid)
            {
                Console.WriteLine($"[WARNING] Login failed: Incorrect password for username '{dto.Username}'.");
                return Unauthorized("Invalid username or password.");
            }

            // Create a token if login is successful
            var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            if (string.IsNullOrEmpty(jwtSecretKey))
            {
                Console.WriteLine("[ERROR] JWT_SECRET_KEY environment variable is not set.");
                return StatusCode(500, "Server configuration error. Please contact support.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, dto.Username),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "procrastination-pastimes",
                audience: "procrastination-pastimes",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            Console.WriteLine($"[INFO] User '{dto.Username}' logged in successfully.");
            return Ok(new { token = tokenString, username = dto.Username });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Error during login for username '{dto.Username}': {ex.Message}");
            return StatusCode(500, "An internal error occurred during login.");
        }
    }
}
