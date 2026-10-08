using System.Reflection;
using Microsoft.Data.Sqlite;

namespace JimmyRewrite.Database;

public static class Database
{
    private const string ConnectionString = "Data Source=jimmy.db;";
    private const int DatabaseVersion = 1;

    private static SqliteConnection GetOpenedSqliteConnection()
    {
        var newConnection = new SqliteConnection(ConnectionString);
        newConnection.Open();
        
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.CheckVersion.sql", newConnection);
        var version = (int)command.ExecuteScalar()!;
        if (version != DatabaseVersion) throw new Exception("Database version mismatch!");
        
        using var command2 = GetSqliteCommand("JimmyRewrite.Database.Statements.Init.sql", newConnection);
        command2.Parameters.AddWithValue("DatabaseVersion", DatabaseVersion);
        command2.ExecuteNonQuery();
        
        return newConnection;
    }

    private static SqliteCommand GetSqliteCommand(string scriptManifestResourceName, SqliteConnection connection)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(scriptManifestResourceName);
        if (stream == null) throw new Exception("Could not find database statements!");
        using var reader = new StreamReader(stream);
        var sql = reader.ReadToEnd();
        
        return new SqliteCommand(sql, connection);
    }
    
    
}