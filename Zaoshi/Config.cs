using System.Text.Json;
using System.Text.Json.Nodes;
using Zaoshi.Exceptions;

namespace Zaoshi;

public static class Config
{
    public static ulong[] testGuilds;
    private static readonly string debugToken = "";
    private static readonly string token = "";
    private static readonly string connectionString = "";

    static Config()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
        if (!File.Exists(path)) throw new FatalException("Cannot find 'config.json' file, make sure it's next to the bot executable");

        var json = JsonNode.Parse(File.ReadAllText(path))!;
        token = json["token"]?.GetValue<string>() ?? throw new Exception("config.json: Missing token");
        debugToken = json["debugToken"]?.GetValue<string>() ?? "";
        testGuilds = json["testGuilds"].Deserialize<ulong[]>() ?? new ulong[]{};
        connectionString = json["connectionString"].Deserialize<string>() ?? throw new Exception("config.json: Missing database connection string. Get one at https://www.mongodb.com/");
    }

    public static string GetToken() => string.IsNullOrEmpty(token) ? throw new Exception("Token not found - add it into your config.json") : token;
    public static string GetDebugToken() => string.IsNullOrEmpty(debugToken) ? throw new Exception("Debug token not found - add it into your config.json") : debugToken;

    public static string GetConnectionString() => string.IsNullOrEmpty(connectionString) ? throw new Exception("This bot needs a MongoDB connection string to function. Get one at https://www.mongodb.com and create your cluster") : connectionString;
}
