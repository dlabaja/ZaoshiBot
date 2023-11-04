using System.Text.Json;
using System.Text.Json.Nodes;

namespace Zaoshi;

/// <summary>
///     Handles credentials and bot settings stored in config.json
/// </summary>
public static class Config
{
    /// <summary>
    ///     Bot token
    /// </summary>
    public static readonly string token;
    /// <summary>
    ///     Second token, use for debugging purposes (can be empty)
    /// </summary>
    public static readonly string debugToken;
    /// <summary>
    ///     Guilds with instant command registration, for testing purposes only
    /// </summary>
    public static readonly ulong[] testGuilds;
    /// <summary>
    ///     Connection string to MongoDB database
    /// </summary>
    public static readonly string connectionString;

    static Config()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
        if (!File.Exists(path)) throw new Exception("Cannot find 'config.json' file, make sure it's next to the bot executable");

        var json = JsonNode.Parse(File.ReadAllText(path))!;
        token = json[nameof(token)]?.GetValue<string>() ?? throw new Exception("config.json: Missing token");
        debugToken = json[nameof(debugToken)]?.GetValue<string>() ?? "";
        testGuilds = json[nameof(testGuilds)].Deserialize<ulong[]>() ?? new ulong[]{};
        connectionString = json[nameof(connectionString)].Deserialize<string>() ?? throw new Exception("config.json: Missing database connection string. Get one at https://www.mongodb.com/ or self-host the database yourself");
    }
}
