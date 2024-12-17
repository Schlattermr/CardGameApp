using Microsoft.AspNetCore.Mvc;
using Accessors;
using System.Collections.Generic;

namespace BackendAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
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
            // Get top 5 leaderboard usernames and wins in descending oder
            var leaderboardData = await _leaderboardAccessor.GrabLeaderboardDataAsync(_connectionString);

            return Ok(leaderboardData);
        }
        catch (Exception e)
        {
            return BadRequest($"Failed to load leaderboard: {e.Message}");
        }
    }
}
