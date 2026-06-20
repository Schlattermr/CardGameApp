using Microsoft.Data.SqlClient;
using System.Data;

namespace Backend.Repository;

public static class DatabaseUtilities
{
    public static string CreateConnectionString()
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = "DESKTOP-CM9UCFI\\MSSQLSERVER01",
            UserID = "DESKTOP-CM9UCFI\\Matthew",
            IntegratedSecurity = true,
            TrustServerCertificate = true
        };
        return builder.ConnectionString;
    }

    public static async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(
        string query, Dictionary<string, object>? parameters, string connectionString)
    {
        var queryResults = new List<Dictionary<string, object>>();

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(query, connection);

        if (parameters != null)
        {
            foreach (var p in parameters)
                command.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
        }

        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object>();
            for (var i = 0; i < reader.FieldCount; i++)
                row[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? DBNull.Value : reader.GetValue(i);
            queryResults.Add(row);
        }

        return queryResults;
    }

    public static async Task<int> ExecuteNonQueryAsync(
        string query, Dictionary<string, object>? parameters, string connectionString)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync() as SqlTransaction
            ?? throw new InvalidOperationException("Failed to begin transaction.");

        try
        {
            await using var command = new SqlCommand(query, connection, transaction);

            if (parameters != null)
            {
                foreach (var p in parameters)
                    command.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
            }

            var rowsAffected = await command.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
            return rowsAffected;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
