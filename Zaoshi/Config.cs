using System.Text.Json;
using System.Text.Json.Nodes;

namespace Zaoshi;

public static class Config
{
#pragma warning disable CS8618
    public static ulong[] testGuilds;
#pragma warning restore CS8618
    private static string debugToken = "";
    private static string token = "";

    public static string GetToken() => string.IsNullOrEmpty(token) ? throw new Exception("Token not found - add it into your config.json") : token;
    public static string GetDebugToken() => string.IsNullOrEmpty(debugToken) ? throw new Exception("Debug token not found - add it into your config.json") : debugToken;

    public static void LoadConfig()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
        if (!File.Exists(path)) throw new Exception("Cannot find 'config.json' file, make sure it's next to the bot executable");

        var json = JsonNode.Parse(File.ReadAllText(path))!;
        token = json["token"]?.GetValue<string>() ?? "";
        debugToken = json["debugToken"]?.GetValue<string>() ?? "";
        testGuilds = json["testGuilds"].Deserialize<ulong[]>() ?? new ulong[]{};
    }
}
