// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 TheMonHub

using System.Text.RegularExpressions;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace JimmyRewrite.events;

public partial class MessageCreateHandler(GatewayClient client) : IMessageCreateGatewayHandler
{
    [GeneratedRegex(@"(?:https?://)?(?:www\.)?(?:discord\.gg|discord(?:app)?\.com/invite)/([a-zA-Z0-9-]+)", RegexOptions.IgnoreCase)]
    private static partial Regex InviteLinkRegex();
    
    public ValueTask HandleAsync(Message arg)
    {
        if (arg.Author.IsBot)
        {
            return default;
        }
        if (arg.GuildId == null)
        {
            return default;
        }
        MessageCacheManager.Get(arg.ChannelId, client.Rest).Add(arg);

        var watchLogChannel = Database.Database.GetLogWatchChannel((ulong)arg.GuildId);
        if (watchLogChannel == null)
        {
            return default;
        }

        if (!Database.Database.GetMemberWatch((ulong)arg.GuildId, arg.Author.Id))
        {
            return default;
        }

        if (arg.Attachments.Count > 0 || InviteLinkRegex().IsMatch(arg.Content))
        {
            // Log here
            return default;
        }

        return default;
    }
    // TODO: Make Watch Log
}