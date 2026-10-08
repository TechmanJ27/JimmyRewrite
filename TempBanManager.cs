using Microsoft.Extensions.Hosting;
using NetCord.Rest;

namespace JimmyRewrite;

public class TempBanManager(RestClient client) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var expiredTBans = Database.Database.RemoveExpiredTBans();

            foreach (var (guildId, userId) in expiredTBans)
            {
                await client.UnbanGuildUserAsync(guildId, userId, cancellationToken: stoppingToken);
                await ModerationHandler.LogTempUnban(guildId, userId, client);
            }
            
            await Task.Delay(60000, stoppingToken);
        }
    }
}