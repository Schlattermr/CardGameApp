namespace Backend.Models.DTOs;

public class UpdateLeaderboardRequest
{
    public required string Username { get; set; }

    public int? Wins { get; set; }
}
