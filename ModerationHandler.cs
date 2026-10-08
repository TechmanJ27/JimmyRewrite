using NetCord;
using NetCord.Rest;

namespace JimmyRewrite;

public static class ModerationHandler
{
    public enum PunishmentType
    {
        Unknown = 0,
        Ban = 1,
        Kick = 2,
        Timeout = 3,
        Warn = 4,
        Appeal = 5
    }
    
    public static void LogConfigChange(ulong guildId, User who, string what, string? from, string to, RestClient restClient)
    {
        var modLogChannel = Database.Database.GetLogModChannel(guildId);
        if (modLogChannel == null)
        {
            return;
        }
        
        var embed = new EmbedProperties()
            .WithTitle("Configuration Changed")
            .WithFields([
                new EmbedFieldProperties()
                    .WithName("User")
                    .WithValue($"<@{who.Id}> ({who.Username} | {who.Id})").WithInline(),
                new EmbedFieldProperties()
                    .WithName("Configuration")
                    .WithValue(what).WithInline(),
                new EmbedFieldProperties()
                    .WithName("From")
                    .WithValue(from ?? "*There was no previous configuration*"),
                new EmbedFieldProperties()
                    .WithName("To")
                    .WithValue(to)
            ]).WithTimestamp(DateTime.UtcNow);
        
        var messageProperties = new MessageProperties().AddEmbeds(embed);

        restClient.SendMessageAsync((ulong)modLogChannel, messageProperties);
    }

    public static async Task LogTempUnban(ulong guildId, ulong who, RestClient client)
    {
        var logModChannel = Database.Database.GetLogModChannel(guildId);
        if (logModChannel == null)
        {
            return;
        }

        var user = await client.GetUserAsync(who);

        var embed = new EmbedProperties()
            .WithTitle("Temporary Ban Expired")
            .WithFields([
                new EmbedFieldProperties()
                    .WithName("User")
                    .WithValue($"<@{who}> ({user.Username} | {who})")
                    .WithInline()
            ])
            .WithTimestamp(DateTime.UtcNow)
            .WithColor(Colors.Blue);
        
        var messageProperties = new MessageProperties().AddEmbeds(embed);
        await client.SendMessageAsync((ulong)logModChannel, messageProperties);
    }

    public static void BanUser(User user, int[] rule, int durationSecond, string? note, string? modNote)
    {
        
    }

    public static void KickUser(User user, int[] rule, string? note, string? modNote)
    {
        
    }

    public static void TimeoutUser(User user, int[] rule, int durationSecond, string? note, string? modNote)
    {
        
    }
    
    public static void WarnUser(User user, int[] rule, string? note, string? modNote)
    {
        
    }
}