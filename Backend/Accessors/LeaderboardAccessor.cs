namespace Accessors;
/*
 *   Responsible for persisting and retrieving leaderboard data, such as 
 *   user wins in solitaire and war.
 */
public class LeaderboardAccessor
{
    /*
     *  Updates wins in the leaderboard
     */
    public async Task UpdateUserWinsAsync(int userId, int gameId, int wins, string connectionString)
    {
        var query = @"UPDATE Leaderboards SET Wins = @Wins 
                    WHERE UserId = @UserId AND GameId = @GameId";
        var parameters = new Dictionary<string, object>
        {
            {"@Wins", wins},
            {"@UserId", userId},
            {"@GameId", gameId}
        };

        await DatabaseUtilities.ExecuteNonQueryAsync(query, parameters, connectionString);
    }

    /*
     *  Grabs top 5 usernames and wins to use on leaderboard in frontend
     */
    public async Task<List<Dictionary<string, object>>?> GrabLeaderboardDataAsync(string connectionString)
    {
        var query = @"
                SELECT TOP 5 u.Username, l.Wins 
                FROM Leaderboards l
                INNER JOIN Users u ON l.UserId = u.UserId
                ORDER BY l.Wins DESC";

        var result = await DatabaseUtilities.ExecuteQueryAsync(query, null, connectionString);
        if(result.Count > 0) 
        {
            return result;
        }
        else
        {
            return null;    // Return null if no user was found
        }
    }
}