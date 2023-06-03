using System.Text.Json.Nodes;

namespace Zaoshi.Utils;

public class JSON
{
    public async static Task<JsonNode> ParseJson(string url)
    {
        HttpResponseMessage response = await new HttpClient().GetAsync(url);
        return JsonNode.Parse(await response.Content.ReadAsStreamAsync())!;
    }
}
