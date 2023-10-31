using System.Text.Json.Nodes;

namespace Zaoshi.Utils;

/// <summary>
///     Utils for work with json format
/// </summary>
public static class Json
{
    /// <summary>
    ///     Parses json from url
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public async static Task<JsonNode> ParseJson(string url)
    {
        var response = await new HttpClient().GetAsync(url);
        return JsonNode.Parse(await response.Content.ReadAsStreamAsync())!;
    }
}
