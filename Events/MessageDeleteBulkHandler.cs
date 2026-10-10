// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 TheMonHub

using System.Net;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace JimmyRewrite.events;

public class MessageDeleteBulkHandler(GatewayClient client) : IMessageDeleteBulkGatewayHandler
{
    public async ValueTask HandleAsync(MessageDeleteBulkEventArgs arg)
    {
        if (arg.GuildId == null)
        {
            return;
        }

        var messageLog = Database.Database.GetLogMessageChannel((ulong)arg.GuildId);
        if (messageLog == null)
        {
            return;
        }

        var messages = new MessageProperties[arg.MessageIds.Count];

        for (var i = 0; i < arg.MessageIds.Count; i++)
        {
            var messageId = arg.MessageIds[i];
            var channelCache = MessageCacheManager.Get(arg.ChannelId, client.Rest);
            var deletedMessage = channelCache.Get(messageId);
            
            if (deletedMessage == null)
            {
                EmbedProperties[] noCacheEmbed =
                [
                    new EmbedProperties()
                        .WithTitle("Message Bulk Deleted")
                        .WithFields([
                            new EmbedFieldProperties()
                                .WithName("User")
                                .WithValue($"*Unknown User*")
                                .WithInline(),
                            new EmbedFieldProperties()
                                .WithName("Link")
                                .WithValue($"https://discord.com/channels/{arg.GuildId}/{arg.ChannelId}/{messageId}")
                        ])
                        .WithTimestamp(DateTime.UtcNow)
                        .WithFooter(new EmbedFooterProperties()
                            .WithText($"ID: {messageId}"))
                        .WithColor(Colors.Red),
                    new EmbedProperties()
                        .WithTitle("Message Content")
                        .WithDescription("*Message are too old and cannot be retrieved*")
                        .WithColor(Colors.Red)
                ];
            
                messages[i] = new MessageProperties().AddEmbeds(noCacheEmbed);
                continue;
            }

            if (deletedMessage.Author.IsBot)
            {
                continue;
            }
        
            EmbedProperties[] embed =
            [
                new EmbedProperties()
                    .WithTitle("Message Bulk Deleted")
                    .WithFields([
                        new EmbedFieldProperties()
                            .WithName("User")
                            .WithValue($"<@{deletedMessage.Author.Id}> ({deletedMessage.Author.Id})")
                            .WithInline(),
                        new EmbedFieldProperties()
                            .WithName("Link")
                            .WithValue($"https://discord.com/channels/{arg.GuildId}/{arg.ChannelId}/{messageId}")
                    ])
                    .WithTimestamp(DateTime.UtcNow)
                    .WithFooter(new EmbedFooterProperties()
                        .WithText($"ID: {messageId}"))
                    .WithColor(Colors.Red),
                new EmbedProperties()
                    .WithTitle("Message Content")
                    .WithDescription(deletedMessage.Content)
                    .WithColor(Colors.Red)
            ];

            messages[i] = new MessageProperties().AddEmbeds(embed).WithContent(ToStringUtil.AttachmentsToString(deletedMessage.Attachments));
        }

        foreach (var message in messages)
        {
            try
            {
                await client.Rest.SendMessageAsync((ulong)messageLog, message);
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
}