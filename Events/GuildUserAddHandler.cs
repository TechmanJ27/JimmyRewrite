// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 TheMonHub

using System.Net;
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
            .WithContent($"<@{arg.Id}> ({arg.Id}) joined the server");

        try
        {
            await client.Rest.SendMessageAsync((ulong)welcomeLog, message);
        }
        catch (RestException e)
        {
            if (e.StatusCode != HttpStatusCode.NotFound && e.StatusCode != HttpStatusCode.Forbidden)
            {
                throw;
            }
        }
    }
}