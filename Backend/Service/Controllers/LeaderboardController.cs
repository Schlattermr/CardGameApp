using Microsoft.AspNetCore.Mvc;
using Backend.Repository;

namespace Backend.Controllers;

[ApiController]
[Route("api/leaderboard")]
public class LeaderboardController() : ControllerBase
{
    private readonly string _connectionString = DatabaseUtilities.CreateConnectionString();

    [HttpGet("all/data")]
    public async Task<IActionResult> GetLeaderboardData()
    {
        try
        {
            // Get leaderboard usernames and wins in descending order
            var leaderboardData = await LeaderboardRepository.GrabLeaderboardDataAsync(_connectionString);
            return Ok(leaderboardData);
        }
        catch (Exception e)
        {
            return BadRequest($"Failed to load leaderboard: {e.Message}");
        }
    }

    [HttpGet("wins")]
    public async Task<IActionResult> GetLeaderboardWins([FromQuery] string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return BadRequest("Invalid request payload.");
        }

        try
        {
            var winsData = await LeaderboardRepository.GrabUserWinsDataAsync(username, _connectionString);
            if (winsData == null)
            {
                return NotFound("User not found.");
            }

            Console.WriteLine($"[INFO] Got wins from leaderboard for user {username}.");
            return Ok(winsData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to get wins from leaderboard: {ex.Message}");
            return StatusCode(500, "An error occurred while getting wins from the leaderboard.");
        }
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateLeaderboardWins([FromBody] UpdateLeaderboardRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Username) || request.Wins < 0)
        {
            return BadRequest("Invalid request payload.");
        }

        try
        {
            await LeaderboardRepository.UpdateUserWinsAsync(request.Username, request.Wins, _connectionString);

            Console.WriteLine($"[INFO] Updated wins in leaderboard for user {request.Username}.");
            return Ok("Leaderboard wins updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to update wins in leaderboard: {ex.Message}");
            return StatusCode(500, "An error occurred while updating wins in the leaderboard.");
        }
    }
}

public class UpdateLeaderboardRequest
{
    public required string Username 
    { 
        get; set; 
    }

    public required int Wins 
    { 
        get; set; 
    }
}
