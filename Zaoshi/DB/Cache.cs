using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;
using Zaoshi.Exceptions;
using static Zaoshi.DB.Collections;

namespace Zaoshi.DB;

/// <summary>
///     Stores the database cache
/// </summary>
public static class Cache
{
    private static readonly Dictionary<MemoryCache, MemoryCacheEntryOptions> cacheToEntryOptions = new Dictionary<MemoryCache, MemoryCacheEntryOptions>();

    static Cache()
    {

    }

    private static MemoryCache SetupCache<T>(int slidingExpirationSecs = 30, int absoluteExpirationSecs = 600) where T : Collections
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var entryOptions = new MemoryCacheEntryOptions{
            SlidingExpiration = TimeSpan.FromSeconds(slidingExpirationSecs),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(absoluteExpirationSecs)
        };

        entryOptions.RegisterPostEvictionCallback((serverId, value, reason, _) =>
        {
            if (reason == EvictionReason.Expired)
                Mongo.GetCollection<T>().ReplaceOne(Builders<BsonDocument>.Filter.Eq("_id", serverId), value.ToBsonDocument());
        });

        cacheToEntryOptions.Add(cache, entryOptions);

        return cache;
    }

    /// <summary>
    ///     Updates data in cache
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="serverId"></param>
    /// <param name="field"></param>
    /// <param name="value"></param>
    public static void UpdateOrAdd<T>(this MemoryCache cache, ulong serverId, string field, object value) where T : Collections, new()
    {
        var data = cache.GetOrFetch<T>(serverId);

        data.Set(field, BsonValue.Create(value));
        cache.Set(serverId, data.ToBsonDocument(), cacheToEntryOptions[cache]);
    }

    /// <summary>
    ///     Gets data from cache or the database
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="serverId"></param>
    /// <returns></returns>
    public static BsonDocument GetOrFetch<T>(this IMemoryCache cache, ulong serverId) where T : Collections, new()
    {
        var i = 0;
        while (i < 3)
        {
            if (cache.TryGetValue(serverId, out BsonDocument? cachedData) && cachedData != null)
                return cachedData;

            try
            {
                var bson = Mongo.GetBsonDocument(typeof(T).Name, serverId);

                if (bson != null)
                {
                    cache.Set(serverId, bson);
                    return bson;
                }
            }
            catch {}

            Mongo.InsertNewToCollection<T>(typeof(T).Name, serverId);
            i++;
        }

        throw new FatalException("Unable to get data from DB");
    }

#pragma warning disable CS1591
    public static readonly MemoryCache ServerSettings = SetupCache<ServerSettings>(300);
    public static readonly MemoryCache Counting = SetupCache<Counting>();
#pragma warning restore CS1591
}
