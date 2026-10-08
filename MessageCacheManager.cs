using System.Collections.Concurrent;
using NetCord.Rest;

namespace JimmyRewrite;

public static class MessageCacheManager
{
    private static readonly ConcurrentDictionary<ulong, MessageCache> Caches = new();

    public static MessageCache Get(ulong channelId, RestClient client)
    {
        return Caches.GetOrAdd(channelId, _ =>
        {
            var returnValue = new MessageCache(channelId, out var warmCacheFunc);
            warmCacheFunc(client);
            return returnValue;
        });
    }

    public static bool Remove(ulong channelId)
    {
        return Caches.TryRemove(channelId, out _);
    }
}