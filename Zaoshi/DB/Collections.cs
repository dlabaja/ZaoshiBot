using MongoDB.Bson;

namespace Zaoshi.Mongo;

public abstract class Collections
{
    public abstract class Counting
    {
        public ObjectId _id { get; set; }
        public long uid { get; set; }
        public long channelId { get; set; }
        public int count { get; set; }
    }
}
