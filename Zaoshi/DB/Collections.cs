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
        public ulong _id;
        public ulong countingChannelId = 0;
        public ulong wordFootballChannelId = 0;
    }

    public class Counting : Collections
    {
        [BsonId]
        public ulong _id;
        public int count = 0;
        public bool isAscending = true;
        public ulong lastUserId = 0;
    }

    public class WordFootball : Collections
    {
        [BsonId]
        public ulong _id;
        public char lastLetter = default;
        public ulong lastUserId = 0;
    }
}
