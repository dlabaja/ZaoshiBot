using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Zaoshi.DB;

/// <summary>
///     Stores the database cache
/// </summary>
public static class Cache
{
    private static readonly Dictionary<MemoryCache, MemoryCacheEntryOptions> cacheToEntryOptions = new Dictionary<MemoryCache, MemoryCacheEntryOptions>();

    private static MemoryCache SetupCache(int slidingExpirationSecs = 30, int absoluteExpirationSecs = 300)
    {
        var cache = new MemoryCache(new MemoryCacheOptions());

        var entryOptions = new MemoryCacheEntryOptions{
            SlidingExpiration = TimeSpan.FromSeconds(slidingExpirationSecs),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(absoluteExpirationSecs)
        };

        cacheToEntryOptions.Add(cache, entryOptions);

        return cache;
    }

    /// <summary>
    ///     Updates one field in cache and saves it to database
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="serverId"></param>
    /// <param name="field"></param>
    /// <param name="value"></param>
    public static void UpdateOrAdd<T>(this MemoryCache cache, ulong serverId, string field, object value) where T : Collections, new() => cache.UpdateOrAdd<T>(serverId, new Dictionary<string, object>{{field, value}});

    /// <summary>
    ///     Updates multiple fields at once in cache and saves it to database
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="serverId"></param>
    /// <param name="fieldsAndValues"></param>
    /// <typeparam name="T"></typeparam>
    public static void UpdateOrAdd<T>(this MemoryCache cache, ulong serverId, Dictionary<string, object> fieldsAndValues) where T : Collections, new()
    {
        var data = cache.GetOrFetch<T>(serverId).ToBsonDocument();

        var updates = new List<UpdateDefinition<BsonDocument>>();
        foreach (var item in fieldsAndValues)
        {
            data.Set(item.Key, BsonValue.Create(item.Value));
            updates.Add(Builders<BsonDocument>.Update.Set(item.Key, BsonValue.Create(item.Value)));
        }

        cache.Set(serverId, data.ToBsonDocument(), cacheToEntryOptions[cache]);
        new Thread(_ => Mongo.GetCollection<T>().UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", serverId), Builders<BsonDocument>.Update.Combine(updates))).Start();
    }

    /// <summary>
    ///     Gets data from cache or the database
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="serverId"></param>
    /// <returns></returns>
    public static T GetOrFetch<T>(this IMemoryCache cache, ulong serverId) where T : Collections, new()
    {
        var i = 0;
        while (i < 3)
        {
            if (cache.TryGetValue(serverId, out BsonDocument? cachedData) && cachedData != null)
                return BsonSerializer.Deserialize<T>(cachedData);

            try
            {
                var bson = Mongo.GetBsonDocument(typeof(T).Name, serverId);

                if (bson != null)
                {
                    cache.Set(serverId, bson);
                    return BsonSerializer.Deserialize<T>(bson);
                }
            }
            catch {}

            Mongo.InsertNewToCollection<T>(typeof(T).Name, serverId);
            i++;
        }

        throw new Exception("Unable to get data from DB");
    }

#pragma warning disable CS1591
    public static readonly MemoryCache ServerSettings = SetupCache(180, 900);
    public static readonly MemoryCache Counting = SetupCache();
    public static readonly MemoryCache WordFootball = SetupCache();
#pragma warning restore CS1591
}
