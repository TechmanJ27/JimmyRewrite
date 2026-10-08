using Microsoft.Data.Sqlite;

namespace JimmyRewrite;

public class Database
{
    public const string ConnectionString = "Data Source=jimmy.db;";

    public Database()
    {
        using var connection = new SqliteConnection(ConnectionString);
        
        connection.Open();
        
    }
}