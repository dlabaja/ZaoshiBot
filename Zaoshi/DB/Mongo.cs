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
        foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                     .SelectMany(a => a.GetTypes())
                     .Where(t => t is{IsClass: true} && t.BaseType == typeof(Collections)))
        {
            var method = typeof(Mongo).GetMethod("GetCollection", BindingFlags.Public | BindingFlags.Static)!.MakeGenericMethod(type);
            var collection = (dynamic)method.Invoke(null, null)!;
            if (!database.ListCollectionNames().ToList().Contains(collection.CollectionNamespace.CollectionName))
            {
                Console.WriteLine($"Creating DB Collection '{collection.CollectionNamespace.CollectionName}'");
                database.CreateCollection(collection.CollectionNamespace.CollectionName);
            }

            // adds missing fields to all bson documents
            foreach (var field in type.GetFields())
            {
                var filter = Builders<BsonDocument>.Filter.Exists(field.Name, false);
                var update = Builders<BsonDocument>.Update.Set(field.Name, (dynamic)Convert.ChangeType(field.GetValue(Activator.CreateInstance(type)), field.FieldType)!);
                collection.UpdateMany(filter, update);
            }
        }
    }

    /// <summary>
    ///     Inserts Bson document to a collection
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="serverId"></param>
    /// <typeparam name="T"></typeparam>
    public static void InsertNewToCollection<T>(string collectionName, ulong serverId) where T : Collections, new()
    {
        var bson = new T().ToBsonDocument().Set("_id", (long)serverId);
        database.GetCollection<BsonDocument>(collectionName).InsertOne(bson);
    }

    /// <summary>
    ///     Gets a MongoDB collection by associated Collections type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IMongoCollection<BsonDocument> GetCollection<T>() where T : Collections => database.GetCollection<BsonDocument>(typeof(T).Name);

    /// <summary>
    ///     Gets Bson document from a collection identified by serverId
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="serverId"></param>
    /// <returns></returns>
    public static BsonDocument? GetBsonDocument(string collectionName, ulong serverId) => database.GetCollection<BsonDocument>(collectionName).Find(Builders<BsonDocument>.Filter.Eq("_id", serverId)).FirstOrDefault();
}
