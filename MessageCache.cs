// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 TheMonHub

using Microsoft.Extensions.Caching.Memory;
using NetCord;
using NetCord.Rest;

namespace JimmyRewrite;

public sealed class MessageCache
{
    private const int Capacity = 1000;
    private readonly ulong _channelId;

    public MessageCache(ulong channelId, out Func<RestClient, Task> warmCacheFunc)
    {
        _channelId = channelId;
        warmCacheFunc = WarmCache;
    }

    private async Task WarmCache(RestClient restClient)
    {
        ulong lastMessageId;
        var channel = await restClient.GetChannelAsync(_channelId);
        
        var textChannel = channel as TextChannel;
        if (textChannel != null && textChannel.LastMessageId != null)
        {
            lastMessageId = textChannel.LastMessageId.Value;
            await WarmCacheStageTwo(restClient, lastMessageId, channel);
            return;
        }

        var voiceChannel = channel as VoiceGuildChannel;
        if (voiceChannel == null)
        {
            return;
        }

        if (voiceChannel.LastMessageId == null)
        {
            return;
        }
        
        lastMessageId = voiceChannel.LastMessageId.Value;
        await WarmCacheStageTwo(restClient, lastMessageId, channel);
    }

    private async Task WarmCacheStageTwo(RestClient restClient, ulong lastMessageId, Channel channel)
    {
        var messages = await restClient.GetMessagesAroundAsync(channel.Id, lastMessageId);
        foreach (var message in messages)
        {
            Add(message);
        }
    }

    private readonly MemoryCache _cache = new(new MemoryCacheOptions
    {
        SizeLimit = Capacity
    });

    private static readonly MemoryCacheEntryOptions EntryOptions = new()
    {
        Size = 1,
        SlidingExpiration = TimeSpan.FromDays(7)
    };

    public async Task<RestMessage?> GetOrFetch(ulong messageId, RestClient client)
    {
        if (_cache.TryGetValue(messageId, out var message) && message != null)
        {
            return (RestMessage)message;
        }

        var fetchedMessage = await client.GetMessageAsync(_channelId, messageId);
        Add(fetchedMessage);
        return fetchedMessage;
    }
    
    public RestMessage? Get(ulong messageId) => _cache.Get<RestMessage?>(messageId);

    public void Add(RestMessage message)
    {
        _cache.Set(message.Id, message, EntryOptions);
    }

    public void Remove(ulong messageId)
    {
        _cache.Remove(messageId);
    }
}