using MongoDB.Bson;
using MongoDB.Driver;
using System.Reflection;

namespace Zaoshi.DB;

/// <summary>
///     Class for managing MongoDB database
/// </summary>
public static class Mongo
{
    private static readonly IMongoDatabase database;

    static Mongo()
    {
        var client = new MongoClient(Config.GetConnectionString());
        database = client.GetDatabase("Zaoshi");

        // create a collection for each DB type
        foreach (var item in AppDomain.CurrentDomain.GetAssemblies()
                     .SelectMany(a => a.GetTypes())
                     .Where(t => t is{IsClass: true, IsAbstract: true} && t.BaseType == typeof(Collections)))
        {
            var method = typeof(Cache).GetMethod("GetCollection", BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(item);
            var collection = (dynamic)method.Invoke(null, null)!;
            if (database.ListCollectionNames().ToList().Contains(collection.CollectionNamespace.CollectionName))
                continue;
            Console.WriteLine($"Creating DB Collection '{collection.CollectionNamespace.CollectionName}'");
            database.CreateCollection(collection.CollectionNamespace.CollectionName);
        }
    }

    public static void AddNewToCollection<T>(string collectionName, ulong serverId) where T : Collections, new()
    {
        var bson = new T().ToBsonDocument().Set("_id", (long)serverId);
        database.GetCollection<BsonDocument>(collectionName).InsertOne(bson);
    }

    public static IMongoCollection<BsonDocument> GetCollection<T>() where T : Collections => database.GetCollection<BsonDocument>(typeof(T).Name);

    public static BsonDocument? GetBsonDocument(string collectionName, ulong serverId) => database.GetCollection<BsonDocument>(collectionName).Find(Builders<BsonDocument>.Filter.Eq("_id", serverId)).FirstOrDefault();
}
