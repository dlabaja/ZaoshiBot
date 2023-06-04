using MongoDB.Driver;
using Zaoshi.Mongo;

namespace Zaoshi.DB;

public static class Mongo
{
    private static readonly MongoClient _client;
    private static readonly IMongoDatabase _database;

    static Mongo()
    {
        _client = new MongoClient(Config.GetConnectionString());
        _database = _client.GetDatabase("Zaoshi");
    }

    private static void BuildDatabase()
    {
        foreach (var item in typeof(Collections).GetMembers())
        {
            var t = item.GetType();
            var collection = typeof(Mongo).GetMethod("GetCollection")!.MakeGenericMethod(t);
            if (collection.Invoke(null, null) == null)
            {
                Console.WriteLine("Collection not present");
            }
        }
    }

    public static IMongoCollection<T> GetCollection<T>() where T : Collections => _database.GetCollection<T>(typeof(T).Name);

    public static void Update()
    {
        // var client = new MongoClient("mongodb://localhost:27017");
        // var database = client.GetDatabase("mojeDatabaze");
        // var kolekce = database.GetCollection<Collections.Counting>("mojeKolekce");
        //
        // var filter = MongoDB.Driver.Builders<Zaoshi>.Filter.Empty;
        // var update = MongoDB.Driver.Builders<Zaoshi>.Update.Set(x => x.Count, 5);
        //
        // var updateResult = kolekce.UpdateMany(filter, update);
        //
        // Console.WriteLine($"Počet aktualizovaných dokumentů: {updateResult.ModifiedCount}");
    }
}
