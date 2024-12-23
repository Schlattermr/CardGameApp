using Microsoft.AspNetCore.Mvc;
using Accessors;
using System.Collections.Generic;
using BackendAPI.Models;

namespace BackendAPI.Controllers;

[ApiController]
[Route("api/leaderboard")]
public class LeaderboardController : ControllerBase
{
    private readonly LeaderboardAccessor _leaderboardAccessor;
    private readonly string _connectionString;

    public LeaderboardController(LeaderboardAccessor leaderboardAccessor)
    {
        _leaderboardAccessor = leaderboardAccessor;
        _connectionString = DatabaseUtilities.CreateConnectionString();
    }

    [HttpGet]
    public async Task<IActionResult> GetLeaderboardData(LeaderboardAccessor _leaderboardAccessor)
    {
        try
        {
            // Get leaderboard usernames and wins in descending oder
            var leaderboardData = await _leaderboardAccessor.GrabLeaderboardDataAsync(_connectionString);

            return Ok(leaderboardData);
        }
        catch (Exception e)
        {
            return BadRequest($"Failed to load leaderboard: {e.Message}");
        }
    }

    [HttpPost("leaderboard/get/wins")]
    public async Task<IActionResult> GetLeaderboardWins(string username)
    {
        if (username == null)
        {
            return BadRequest("Invalid request payload.");
        }

        try
        {
            string connectionString = DatabaseUtilities.CreateConnectionString();
            LeaderboardAccessor leaderboardAccessors = new LeaderboardAccessor();
            await leaderboardAccessors.GrabUserWinsDataAsync(username, connectionString);

            Console.WriteLine($"[INFO] Got wins from leaderboard for user {username}.");
            return Ok("Leaderboard wins grabbed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to get wins from leaderboard: {ex.Message}");
            return StatusCode(500, "An error occurred while getting wins from the leaderboard.");
        }
    }

    [HttpPost("leaderboard/update")]
    public async Task<IActionResult> UpdateLeaderboardWins(string username, int wins)
    {
        if (wins == 0)
        {
            return BadRequest("Invalid request payload.");
        }

        try
        {
            string connectionString = DatabaseUtilities.CreateConnectionString();
            LeaderboardAccessor leaderboardAccessors = new LeaderboardAccessor();
            await leaderboardAccessors.UpdateUserWinsAsync(username, wins, connectionString);

            Console.WriteLine($"[INFO] Got wins from leaderboard for user {username}.");
            return Ok("Leaderboard wins grabbed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to get wins from leaderboard: {ex.Message}");
            return StatusCode(500, "An error occurred while getting wins from the leaderboard.");
        }
    }
}
