namespace Backend.Models.DTOs;

public class UpdateLeaderboardRequest
{
    public required string Username { get; set; }

    public required int Wins { get; set; }
}
