using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace JimmyRewrite.events;

public class GuildUserRemoveHandler(GatewayClient client) : IGuildUserRemoveGatewayHandler
{
    public async ValueTask HandleAsync(GuildUserRemoveEventArgs arg)
    {
    }
}