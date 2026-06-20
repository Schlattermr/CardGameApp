namespace Backend.Repository;

public static class UserAccessor
{
    public static async Task AddNewUserAsync(string username, string password, string connectionString)
    {
        var query = @"INSERT INTO Users (Username, PasswordHash)
                    SELECT @Username, @PasswordHash
                    WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Username = @Username)";
        var parameters = new Dictionary<string, object>
        {
            {"@Username", username},
            {"@PasswordHash", password}
        };

        await DatabaseUtilities.ExecuteNonQueryAsync(query, parameters, connectionString);
    }

    public static async Task<int?> GetUserIdAsync(string username, string connectionString)
    {
        var query = "SELECT UserId FROM Users WHERE Username = @Username";
        var parameters = new Dictionary<string, object>
        {
            {"@Username", username}
        };

        var result = await DatabaseUtilities.ExecuteQueryAsync(query, parameters, connectionString);

        if (result.Count > 0)
            return (int?)result[0]["UserId"];
        else
            return null;
    }

    public static async Task<string?> GetPasswordHashAsync(int userId, string connectionString)
    {
        var query = "SELECT PasswordHash FROM Users WHERE UserId = @UserId";
        var parameters = new Dictionary<string, object>
        {
            { "@UserId", userId }
        };

        var result = await DatabaseUtilities.ExecuteQueryAsync(query, parameters, connectionString);

        if (result.Count > 0)
            return (string)result[0]["PasswordHash"];
        else
            return null;
    }
}
