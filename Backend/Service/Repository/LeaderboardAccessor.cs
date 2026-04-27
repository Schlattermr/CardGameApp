namespace Backend.Repository;
/*
 *   Responsible for persisting and retrieving leaderboard data, such as 
 *   user wins in solitaire and war.
 */
public class LeaderboardAccessor
{
    /*
     *  Updates wins in the leaderboard
     */
    public async Task UpdateUserWinsAsync(string username, int wins, string connectionString)
    {
        wins++;
        var query = @"UPDATE Leaderboards SET Wins = @Wins 
                      FROM Leaderboards l
                      INNER JOIN Users u ON l.UserId = u.UserId
                      WHERE u.Username = @Username";
        var parameters = new Dictionary<string, object>
        {
            {"@Wins", wins},
            {"@Username", username}
        };

        await DatabaseUtilities.ExecuteNonQueryAsync(query, parameters, connectionString);
    }

    /*
     *  Grabs user wins from username
     */
    public async Task<List<Dictionary<string, object>>?> GrabUserWinsDataAsync(string username, string connectionString)
    {
        var query = @"SELECT l.Wins FROM Leaderboards l
                      INNER JOIN Users u ON l.UserId = u.UserId
                      WHERE u.Username = @Username";
        var parameters = new Dictionary<string, object>
        {
            {"@Username", username}
        };
        var result = await DatabaseUtilities.ExecuteQueryAsync(query, parameters, connectionString);
        if (result.Count > 0)
        {
            return result;
        }
        else
        {
            return null;    // Return null if no user was found
        }
    }

    /*
     *  Grabs top 7 usernames and wins to use on leaderboard in frontend
     */
    public async Task<List<Dictionary<string, object>>?> GrabLeaderboardDataAsync(string connectionString)
    {
        var query = @"SELECT TOP 7 u.Username, l.Wins 
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
