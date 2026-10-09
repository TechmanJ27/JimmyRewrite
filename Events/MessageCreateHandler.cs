// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 TheMonHub

using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace JimmyRewrite.events;

public class MessageCreateHandler(GatewayClient client) : IMessageCreateGatewayHandler
{
    public ValueTask HandleAsync(Message arg)
    {
        MessageCacheManager.Get(arg.ChannelId, client.Rest).Add(arg);
        return default;
    }
    // TODO: Make Watch Log
}