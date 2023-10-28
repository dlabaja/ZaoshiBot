using MongoDB.Bson.Serialization.Attributes;

#pragma warning disable 1591

namespace Zaoshi.DB;

/// <summary>
///     All of MongoDB Collections types
/// </summary>
public abstract class Collections
{
    public class ServerSettings : Collections
    {
        [BsonId]
        public ulong _id { get; set; }

        public ulong CountingChannelId { get; set; } = 0;
    }

    public class Counting : Collections
    {
        [BsonId]
        public ulong _id { get; set; }

        public int Count { get; set; } = 0;
        public bool IsAscending { get; set; } = true;
    }
}
