using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;

namespace Accessors;
public class DatabaseUtilities 
{
    /*
     *  Creates connection string from Azure database information
     */
    public static string CreateConnectionString() 
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = "csce-361.database.windows.net",
            UserID = "schlatter",
            Password = "Password123###",
            InitialCatalog ="CSCE361"
        };

        var connectionString = builder.ConnectionString;

        return connectionString;
    }

    /*
     *  Utility function to perform a query command in the database, such as SELECT
     */
    public static async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(
    string query, Dictionary<string, object>? parameters, string connectionString)
    {
        // Parameters will be used to prevent SQLi attacks, and ? is used to specify NULL as a possibility
        var queryResults = new List<Dictionary<string, object>>();

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        try
        {
            await using var command = new SqlCommand(query, connection);

            // Add parameters to the command
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    command.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
                }
            }

            // Use CommandBehavior.CloseConnection to ensure the connection is closed after the reader is disposed
            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i);
                }
                queryResults.Add(row);
            }
        }
        catch (Exception)
        {
            throw;
        }

        return queryResults;
    }

    /* 
     *  Utility function to perform a non-query in the database, such as INSERT and DELETE.
     *  Returns the number of rows affected by the command
     */
    public static async Task<int> ExecuteNonQueryAsync(
    string query, Dictionary<string, object>? parameters, string connectionString)
    {
        // Parameters will be used to prevent SQLi attacks, and ? is used to specify NULL as a possibility
        var rowsAffected = 0;

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var transaction = connection.BeginTransaction();

        try
        {
            await using var command = new SqlCommand(query, connection, transaction);

            // Add parameters to the command
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    command.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
                }
            }

            rowsAffected = await command.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

        return rowsAffected;
    }
}