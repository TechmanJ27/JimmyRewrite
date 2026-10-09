using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace JimmyRewrite.events;

public class MessageDeleteHandler(GatewayClient client) : IMessageDeleteGatewayHandler
{
    public async ValueTask HandleAsync(MessageDeleteEventArgs arg)
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

        var channelCache = MessageCacheManager.Get(arg.ChannelId, client.Rest);
        var deletedMessage = channelCache.Get(arg.MessageId);

        if (deletedMessage == null)
        {
            EmbedProperties[] noCacheEmbed =
            [
                new EmbedProperties()
                    .WithTitle("Message Deleted")
                    .WithFields([
                        new EmbedFieldProperties()
                            .WithName("User")
                            .WithValue($"*Unknown User*")
                            .WithInline(),
                        new EmbedFieldProperties()
                            .WithName("Link")
                            .WithValue($"https://discord.com/channels/{arg.GuildId}/{arg.ChannelId}/{arg.MessageId}")
                    ])
                    .WithTimestamp(DateTime.UtcNow)
                    .WithFooter(new EmbedFooterProperties()
                        .WithText($"ID: {arg.MessageId}"))
                    .WithColor(Colors.Red),
                new EmbedProperties()
                    .WithTitle("Message Content")
                    .WithDescription("*Message are too old and cannot be retrieved*")
                    .WithColor(Colors.Red)
            ];
            
            var noCacheMessage = new MessageProperties().AddEmbeds(noCacheEmbed);
        
            await client.Rest.SendMessageAsync((ulong)messageLog, noCacheMessage);
            return;
        }

        if (deletedMessage.Author.IsBot)
        {
            return;
        }
        
        EmbedProperties[] embed =
        [
            new EmbedProperties()
                .WithTitle("Message Deleted")
                .WithFields([
                    new EmbedFieldProperties()
                        .WithName("User")
                        .WithValue($"<@{deletedMessage.Author.Id}> ({deletedMessage.Author.Username} | {deletedMessage.Author.Id})")
                        .WithInline(),
                    new EmbedFieldProperties()
                        .WithName("Link")
                        .WithValue($"https://discord.com/channels/{arg.GuildId}/{arg.ChannelId}/{arg.MessageId}")
                ])
                .WithTimestamp(DateTime.UtcNow)
                .WithFooter(new EmbedFooterProperties()
                    .WithText($"ID: {arg.MessageId}"))
                .WithColor(Colors.Red),
            new EmbedProperties()
                .WithTitle("Message Content")
                .WithDescription(deletedMessage.Content)
                .WithColor(Colors.Red)
        ];
        
        var message = new MessageProperties().AddEmbeds(embed).WithContent(ToStringUtil.AttachmentsToString(deletedMessage.Attachments));

        try
        {
            await client.Rest.SendMessageAsync((ulong)messageLog, message);
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