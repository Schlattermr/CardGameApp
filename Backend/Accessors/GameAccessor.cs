namespace Accessors;
/* 
 *  Responsible for persisting and retrieving game states, such as 
 *  saved Solitaire games or current score and deck status in War.
 */ 
public class GameAccessor
{
    /*
     * Add a new game to the database
     */
    public async Task<int> AddNewGameAsync(string gameName, string connectionString)
    {
        var query = "INSERT INTO Games (GameName) VALUES (@GameName); SELECT SCOPE_IDENTITY();";
        var parameters = new Dictionary<string, object>
        {
            { "@GameName", gameName }
        };

        // Execute query and return the new GameId
        var result = await DatabaseUtilities.ExecuteQueryAsync(query, parameters, connectionString);
        if (result.Count > 0)
        {
            return Convert.ToInt32(result[0]["SCOPE_IDENTITY()"]);
        }
        else
        {
            return -1; // Return -1 if the game was not added successfully
        }
    }

    /*
     * Function to get a game by GameId
     */
    public async Task<Dictionary<string, object>?> GetGameByIdAsync(int gameId, string connectionString)
    {
        var query = "SELECT * FROM Games WHERE GameId = @GameId";
        var parameters = new Dictionary<string, object>
        {
            { "@GameId", gameId }
        };

        var result = await DatabaseUtilities.ExecuteQueryAsync(query, parameters, connectionString);
        if (result.Count > 0)
        {
            return result[0]; // Return the first result as it's expected to be unique
        } 
        else 
        {
            return null; // Return null if no game was found
        }
    }

    /*
     * Get a game by GameName
     */
    public async Task<Dictionary<string, object>?> GetGameByNameAsync(string gameName, string connectionString)
    {
        var query = "SELECT * FROM Games WHERE GameName = @GameName";
        var parameters = new Dictionary<string, object>
        {
            { "@GameName", gameName }
        };

        var result = await DatabaseUtilities.ExecuteQueryAsync(query, parameters, connectionString);
        if (result.Count > 0)
        {
            return result[0]; // Return the first result as it's expected to be unique
        }
        else
        {
            return null;    // Return null if no game was found
        }
    }
}
