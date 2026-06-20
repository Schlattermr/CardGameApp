namespace Backend.Repository;

public static class GameAccessor
{
    public static async Task<int> AddNewGameAsync(string gameName, string connectionString)
    {
        var query = "INSERT INTO Games (GameName) VALUES (@GameName); SELECT SCOPE_IDENTITY();";
        var parameters = new Dictionary<string, object>
        {
            { "@GameName", gameName }
        };

        var result = await DatabaseUtilities.ExecuteQueryAsync(query, parameters, connectionString);
        if (result.Count > 0)
            return Convert.ToInt32(result[0]["SCOPE_IDENTITY()"]);
        else
            return -1;
    }

    public static async Task<Dictionary<string, object>?> GetGameByIdAsync(int gameId, string connectionString)
    {
        var query = "SELECT * FROM Games WHERE GameId = @GameId";
        var parameters = new Dictionary<string, object>
        {
            { "@GameId", gameId }
        };

        var result = await DatabaseUtilities.ExecuteQueryAsync(query, parameters, connectionString);
        return result.Count > 0 ? result[0] : null;
    }

    public static async Task<Dictionary<string, object>?> GetGameByNameAsync(string gameName, string connectionString)
    {
        var query = "SELECT * FROM Games WHERE GameName = @GameName";
        var parameters = new Dictionary<string, object>
        {
            { "@GameName", gameName }
        };

        var result = await DatabaseUtilities.ExecuteQueryAsync(query, parameters, connectionString);
        return result.Count > 0 ? result[0] : null;
    }
}
