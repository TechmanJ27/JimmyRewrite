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

        try
        {
            await client.Rest.SendMessageAsync((ulong)welcomeLog, message);
        }
        catch (RestException e)
        {
            if (e.Error == null || e.Error.Code != 404 && e.Error.Code != 403)
            {
                throw;
            }
        }
    }
}