using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace JimmyRewrite.events;

public class GuildUserRemoveHandler(GatewayClient client) : IGuildUserRemoveGatewayHandler
{
    public async ValueTask HandleAsync(GuildUserRemoveEventArgs arg)
    {
        var welcomeLog = Database.Database.GetLogWelcomeChannel(arg.GuildId);
        if (welcomeLog == null)
        {
            return;
        }

        var message = new MessageProperties()
            .WithContent($"<@{arg.User.Id}> ({arg.User.Username} | {arg.User.Id}) left the server\n");

        await client.Rest.SendMessageAsync((ulong)welcomeLog, message);
    }
}