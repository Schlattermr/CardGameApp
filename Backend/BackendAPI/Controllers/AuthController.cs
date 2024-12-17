using BackendAPI.Data;
using BackendAPI.DTOs;
using BackendAPI.Models;
using Engines;
using Accessors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

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

            // Verify the password using bcrypt
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, passwordHash);
            if (!isPasswordValid)
            {
                Console.WriteLine($"[WARNING] Login failed: Incorrect password for username '{dto.Username}'.");
                return Unauthorized("Invalid username or password.");
            }

            Console.WriteLine($"[INFO] User '{dto.Username}' logged in successfully.");
            return Ok("Login successful.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Error during login for username '{dto.Username}': {ex.Message}");
            return StatusCode(500, "An internal error occurred during login.");
        }
    }
}
