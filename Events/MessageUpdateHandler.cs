// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 TheMonHub

using System.Net;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace JimmyRewrite.events;

public class MessageUpdateHandler(GatewayClient client) : IMessageUpdateGatewayHandler
{
    public async ValueTask HandleAsync(Message arg)
    {
        if (arg.Author.IsBot)
        {
            return;
        }
        if (arg.GuildId == null)
        {
            return;
        }
        var messageLog = Database.Database.GetLogMessageChannel((ulong)arg.GuildId);
        if (messageLog == null)
        {
            return;
        }
        
        var channelCache = MessageCacheManager.Get(arg.ChannelId, client.Rest);
        var oldMessage = channelCache.Get(arg.Id);
        
        channelCache.Add(arg);

        if (oldMessage == null)
        {
            EmbedProperties[] noCacheEmbed =
            [
                new EmbedProperties()
                    .WithTitle("Message Edited")
                    .WithFields([
                        new EmbedFieldProperties()
                            .WithName("User")
                            .WithValue($"<@{arg.Author.Id}> ({arg.Author.Username} | {arg.Author.Id})")
                            .WithInline(),
                        new EmbedFieldProperties()
                            .WithName("Link")
                            .WithValue($"https://discord.com/channels/{arg.GuildId}/{arg.ChannelId}/{arg.Id}")
                    ])
                    .WithTimestamp(DateTime.UtcNow)
                    .WithFooter(new EmbedFooterProperties()
                        .WithText($"ID: {arg.Id}"))
                    .WithColor(Colors.Yellow),
                new EmbedProperties()
                    .WithTitle("Old Message Content")
                    .WithDescription("*Message are too old and cannot be retrieved*")
                    .WithColor(Colors.Yellow),
                new EmbedProperties()
                    .WithTitle("New Message Content")
                    .WithDescription(arg.Content)
                    .WithColor(Colors.Yellow)
            ];
            
            var noCacheMessage = new MessageProperties().AddEmbeds(noCacheEmbed);
            
            await client.Rest.SendMessageAsync((ulong)messageLog, noCacheMessage);
            return;
        }
        
        var urls1 = oldMessage.Attachments.Select(x => x.Url).ToHashSet();
        var urls2 = arg.Attachments.Select(x => x.Url).ToHashSet();

        var added = arg.Attachments
            .Where(x => !urls1.Contains(x.Url))
            .ToList();

        var removed = oldMessage.Attachments
            .Where(x => !urls2.Contains(x.Url))
            .ToList();

        if (added.Count == 0 && removed.Count == 0 && oldMessage.Content.Equals(arg.Content))
        {
            return;
        }
        
        EmbedProperties[] embed =
        [
            new EmbedProperties()
                .WithTitle("Message Edited")
                .WithFields([
                    new EmbedFieldProperties()
                        .WithName("User")
                        .WithValue($"<@{arg.Author.Id}> ({arg.Author.Id})")
                        .WithInline(),
                    new EmbedFieldProperties()
                        .WithName("Link")
                        .WithValue($"https://discord.com/channels/{arg.GuildId}/{arg.ChannelId}/{arg.Id}")
                ])
                .WithTimestamp(DateTime.UtcNow)
                .WithFooter(new EmbedFooterProperties()
                    .WithText($"ID: {arg.Id}"))
                .WithColor(Colors.Yellow),
            new EmbedProperties()
                .WithTitle("Old Message Content")
                .WithDescription(oldMessage.Content)
                .WithColor(Colors.Yellow),
            new EmbedProperties()
                .WithTitle("New Message Content")
                .WithDescription(arg.Content)
                .WithColor(Colors.Yellow)
        ];

        var attachmentSummary = "";
        if (added.Count > 0)
        {
            attachmentSummary = $"**Attachment Added**: {ToStringUtil.AttachmentsToString(added)}";
        }

        if (removed.Count > 0)
        {
            attachmentSummary += $"\n**Attachment Removed**: {ToStringUtil.AttachmentsToString(removed)}";
        }
            
        var message = new MessageProperties().AddEmbeds(embed).WithContent(attachmentSummary);
            
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