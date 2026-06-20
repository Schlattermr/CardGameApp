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
    public async Task<IActionResult> Register(Register dto)
    {
        Console.WriteLine($"[INFO] Registration request received for username: {dto.Username}");
        try
        {
            var existingUserId = await UserAccessor.GetUserIdAsync(dto.Username, _connectionString);
            if (existingUserId.HasValue)
            {
                Console.WriteLine($"[WARNING] Registration failed: Username '{dto.Username}' already exists.");
                return BadRequest("Username already exists.");
            }

            if (!Regex.IsMatch(dto.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                Console.WriteLine($"[WARNING] Registration failed: Password for username '{dto.Username}' does not meet requirements.");
                return BadRequest("Password must be at least 8 characters long and contain one uppercase and one lowercase letter, one number, and one special character (@$!%*?&).");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            await UserAccessor.AddNewUserAsync(dto.Username, hashedPassword, _connectionString);

            Console.WriteLine($"[INFO] User '{dto.Username}' registered successfully.");
            return Ok("User registered successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Error during registration for username '{dto.Username}': {ex.Message}");
            return StatusCode(500, "An internal error occurred during registration.");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(Login dto)
    {
        Console.WriteLine($"[INFO] Login request received for username: {dto.Username}");
        try
        {
            var userId = await UserAccessor.GetUserIdAsync(dto.Username, _connectionString);
            if (!userId.HasValue)
            {
                Console.WriteLine($"[WARNING] Login failed: Username '{dto.Username}' not found.");
                return Unauthorized("Invalid username or password.");
            }

            var passwordHash = await UserAccessor.GetPasswordHashAsync((int)userId, _connectionString);
            if (string.IsNullOrEmpty(passwordHash))
            {
                Console.WriteLine($"[ERROR] Password hash for user '{dto.Username}' is null or empty.");
                return Unauthorized("Invalid username or password.");
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, passwordHash);
            if (!isPasswordValid)
            {
                Console.WriteLine($"[WARNING] Login failed: Incorrect password for username '{dto.Username}'.");
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
                new Claim(ClaimTypes.Name, dto.Username),
                new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()),
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
