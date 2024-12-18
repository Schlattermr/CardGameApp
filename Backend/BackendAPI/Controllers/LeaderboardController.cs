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

    [HttpPost("leaderboard/update")]
    public async Task<IActionResult> UpdateLeaderboardWins([FromBody] Leaderboard request)
    {
        if (request == null || request.UserId <= 0 || request.GameId <= 0 || request.Wins < 0)
        {
            return BadRequest("Invalid request payload.");
        }

        try
        {
            string connectionString = DatabaseUtilities.CreateConnectionString();
            LeaderboardAccessor leaderboardAccessors = new LeaderboardAccessor();
            await leaderboardAccessors.UpdateUserWinsAsync(request.UserId, request.GameId, request.Wins, connectionString);

            Console.WriteLine($"[INFO] Updated leaderboard for user {request.UserId}.");
            return Ok("Leaderboard updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to update leaderboard: {ex.Message}");
            return StatusCode(500, "An error occurred while updating the leaderboard.");
        }
    }
}
