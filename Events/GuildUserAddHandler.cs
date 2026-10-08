using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace JimmyRewrite.events;

public class GuildUserAddHandler(GatewayClient client) : IGuildUserAddGatewayHandler
{
    public async ValueTask HandleAsync(GuildUser arg)
    {
        var welcomeLog = Database.Database.GetLogWelcomeChannel(arg.GuildId);
        if (welcomeLog == null)
        {
            return;
        }

        var message = new MessageProperties()
            .WithContent($"<@{arg.Id}> ({arg.Username} | {arg.Id}) joined the server");

        await client.Rest.SendMessageAsync((ulong)welcomeLog, message);
    }
}