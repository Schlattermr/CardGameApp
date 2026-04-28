namespace Backend.Repository;

/*
 *  Handles all data operations related to user information, such as retrieving user profiles, 
 *  creating accounts, validating login credentials, and managing user sessions.
 */  
public static class UserRepository
{
    /*
     *  Add a new user to database
     *  Returns the number of rows affected (1 if successful, 0 if username already exists)
     */
    public static async Task<int> AddNewUserAsync(string username, string password, string connectionString) 
    {
        // Insert and check the username doesn't already exist
        var query = @"INSERT INTO Users (Username, PasswordHash) 
                    SELECT @Username, @PasswordHash 
                    WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Username = @Username)";
        var parameters = new Dictionary<string, object>
        {
            {"@Username", username},
            {"@PasswordHash", password}
        };

        return await DatabaseUtilities.ExecuteNonQueryAsync(query, parameters, connectionString);
    }

    /*
     *  Gets userId from username
     */
    public static async Task<int?> GetUserIdAsync(string username, string connectionString)
    {
        var query = "SELECT UserId FROM Users WHERE Username = @Username";
        var parameters = new Dictionary<string, object>
        {
            {"@Username", username}
        };

        var result = await DatabaseUtilities.ExecuteQueryAsync(query, parameters, connectionString);

        if(result.Count > 0) {
            return (int?)result[0]["UserId"];
        } else {
            return null;    // Return null if there is no user with that username
        }
    }

    /*
     *  Gets PasswordHash for login verification
     */  
    public static async Task<string?> GetPasswordHashAsync(int userId, string connectionString)
    {
        var query = "SELECT PasswordHash FROM Users WHERE UserId = @UserId";
        var parameters = new Dictionary<string, object> 
        { 
            { "@UserId", userId } 
        };

        var result = await DatabaseUtilities.ExecuteQueryAsync(query, parameters, connectionString);

        if (result.Count > 0)
        {
            return (string)result[0]["PasswordHash"];
        }
        else
        {
            return null;
        }
    }
}
