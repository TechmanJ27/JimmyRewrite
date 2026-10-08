using System.Reflection;
using Microsoft.Data.Sqlite;

namespace JimmyRewrite.Database;

public static class Database
{
    private const string ConnectionString = "Data Source=jimmy.db;";
    private const long DatabaseVersion = 1;

    private static SqliteConnection GetOpenedSqliteConnection()
    {
        var newConnection = new SqliteConnection(ConnectionString);
        newConnection.Open();
        
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.CheckVersion.sql", newConnection);
        var version = (long)command.ExecuteScalar()!;
        if (version != DatabaseVersion && version != 0) throw new Exception("Database version mismatch!");
        
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

    private static void CreateGuildDatIfNotExists(ulong guildId, SqliteConnection connection)
    {
        using var makeGuildTable = GetSqliteCommand("JimmyRewrite.Database.Statements.CreateGuildDatIfNotExists.sql", connection);
        makeGuildTable.Parameters.AddWithValue("GuildId", guildId);
        makeGuildTable.ExecuteNonQuery();
    }

    private static void CreateMemberIfNotExists(ulong guildId, ulong userId, SqliteConnection connection)
    {
        using var makeMemberTable = GetSqliteCommand("JimmyRewrite.Database.Statements.CreateMemberIfNotExists.sql", connection);
        makeMemberTable.Parameters.AddWithValue("GuildId", guildId);
        makeMemberTable.Parameters.AddWithValue("UserId", userId);
        makeMemberTable.ExecuteNonQuery();
    }
    
    public static void SetLogMessageChannel(ulong channelId, ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.SetLogMessageChannel.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.Parameters.AddWithValue("ChannelId", channelId);
        command.ExecuteNonQuery();
    }
    
    public static void SetLogWatchChannel(ulong channelId, ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.SetLogWatchChannel.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.Parameters.AddWithValue("ChannelId", channelId);
        command.ExecuteNonQuery();
    }
    
    public static void SetLogModChannel(ulong channelId, ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.SetLogModChannel.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.Parameters.AddWithValue("ChannelId", channelId);
        command.ExecuteNonQuery();   
    }
    
    public static void SetAppealLink(string text, ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.SetAppealLink.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.Parameters.AddWithValue("Text", text);
        command.ExecuteNonQuery();
    }

    public static void SetForceNote(bool value, ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.SetForceNote.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.Parameters.AddWithValue("Value", value ? 1 : 0);
        command.ExecuteNonQuery();
    }

    public static void SetAllowCustomRule(bool value, ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.SetAllowCustomRule.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.Parameters.AddWithValue("Value", value ? 1 : 0);
        command.ExecuteNonQuery();
    }

    public static void SetModActionConfirm(bool value, ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.SetModActionConfirm.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.Parameters.AddWithValue("Value", value ? 1 : 0);
        command.ExecuteNonQuery();
    }

    public static string? GetAppealLink(ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.GetAppealLink.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        var result = command.ExecuteScalar();
        if (result == DBNull.Value || result == null) return null;
        return (string?)result;
    }

    public static bool GetForceNote(ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.GetForceNote.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        var result = command.ExecuteScalar();
        return (long?)result == 1;
    }

    public static bool GetAllowCustomRule(ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.GetAllowCustomRule.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        var result = command.ExecuteScalar();
        return (long?)result == 1;
    }

    public static bool GetModActionConfirm(ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.GetModActionConfirm.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        var result = command.ExecuteScalar();
        return (long?)result == 1;
    }
    
    public static ulong? GetLogMessageChannel(ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.GetLogMessageChannel.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        var result = command.ExecuteScalar();
        if (result == DBNull.Value) return null;
        return (ulong?)(long?)result;
    }
    
    public static ulong? GetLogWatchChannel(ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.GetLogWatchChannel.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        var result = command.ExecuteScalar();
        if (result == DBNull.Value) return null;
        return (ulong?)(long?)result;
    }
    
    public static ulong? GetLogModChannel(ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.GetLogModChannel.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        var result = command.ExecuteScalar();
        if (result == DBNull.Value) return null;
        return (ulong?)(long?)result;
    }

    public static void ResetLogMessageChannel(ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.ResetLogMessageChannel.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.ExecuteNonQuery();
    }

    public static void ResetLogWatchChannel(ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.ResetLogWatchChannel.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.ExecuteNonQuery();
    }

    public static void ResetLogModChannel(ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.ResetLogModChannel.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.ExecuteNonQuery();
    }

    public static void ResetAppealLink(ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.ResetAppealLink.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.ExecuteNonQuery();
    }

    public static void SetLogWelcomeChannel(ulong channelId, ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.SetLogWelcomeChannel.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.Parameters.AddWithValue("ChannelId", channelId);
    }

    public static void ResetLogWelcomeChannel(ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.ResetLogWelcomeChannel.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.ExecuteNonQuery();
    }

    public static ulong? GetLogWelcomeChannel(ulong guildId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.GetLogWelcomeChannel.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        var result = command.ExecuteScalar();
        if (result == DBNull.Value) return null;
        return (ulong?)(long?)result;
    }
    
    public static void SetMemberWatch(ulong guildId, ulong userId, bool value)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        CreateMemberIfNotExists(guildId, userId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.SetMemberWatch.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.Parameters.AddWithValue("UserId", userId);
        command.Parameters.AddWithValue("Watch", value ? 1 : 0);
        command.ExecuteNonQuery();
    }

    public static bool GetMemberWatch(ulong guildId, ulong userId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        CreateMemberIfNotExists(guildId, userId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.GetMemberWatch.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.Parameters.AddWithValue("UserId", userId);
        var result = command.ExecuteScalar();
        return (long?)result == 1;
    }
    
    public static void AddTBan(ulong guildId, ulong userId, long expire)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.AddTBan.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.Parameters.AddWithValue("UserId", userId);
        command.Parameters.AddWithValue("Expire", expire);
        command.ExecuteNonQuery();
    }

    public static void AddCase(ulong caseId, ulong guildId, ulong userId, ModerationHandler.PunishmentType type, int[] ruleId, ulong modUid, string note, long time, long tBanExpire)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.AddCase.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.Parameters.AddWithValue("UserId", userId);
        command.Parameters.AddWithValue("CaseId", caseId);
        command.Parameters.AddWithValue("Type", (int)type);
        command.Parameters.AddWithValue("RuleId", string.Join(",", ruleId));
        command.Parameters.AddWithValue("RuleTitle", 0); // TODO
        command.Parameters.AddWithValue("ModUid", modUid);
        command.Parameters.AddWithValue("Note", note);
        command.Parameters.AddWithValue("Time", time);
        command.Parameters.AddWithValue("TBanExpire", tBanExpire);
        command.ExecuteNonQuery();
    }

    public static List<(ulong GuildId, ulong UserId)> RemoveExpiredTBans()
    {
        using var connection = GetOpenedSqliteConnection();
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.RemoveExpiredTBans.sql", connection);
        command.Parameters.AddWithValue("Time", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        var reader = command.ExecuteReader();
        
        var results = new List<(ulong GuildId, ulong UserId)>();
        while (reader.Read())
        {
            var gid = (ulong)reader.GetInt64(0);
            var uid = (ulong)reader.GetInt64(1);
            results.Add((gid, uid));
        }

        return results;
    }
    
    public static long GetTBanStatus(ulong guildId, ulong userId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.GetTBanStatus.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.Parameters.AddWithValue("UserId", userId);
        var result = command.ExecuteScalar();
        return (long?)result ?? 0;
    }
    
    public static void RemoveTBan(ulong guildId, ulong userId)
    {
        using var connection = GetOpenedSqliteConnection();
        CreateGuildDatIfNotExists(guildId, connection);
        using var command = GetSqliteCommand("JimmyRewrite.Database.Statements.RemoveTBan.sql", connection);
        command.Parameters.AddWithValue("GuildId", guildId);
        command.Parameters.AddWithValue("UserId", userId);
        command.ExecuteNonQuery();
    }
    
    // TODO: Add rules stuff for database
}