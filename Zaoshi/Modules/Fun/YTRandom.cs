using Discord.Interactions;
using System.Text.Json.Nodes;
using Zaoshi.Utils;

namespace Zaoshi.Modules.Fun;

public class YTRandom : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("yt-random", "Sends a random (and most likely pretty weird) YouTube video")]
    public async Task Command()
    {
        var video = await GetVideo();
        await RespondAsync(@$"**Views: **{video["viewCount"]?.GetValue<int>()}
**Published: **{DateTimeOffset.FromUnixTimeSeconds(video["published"]!.GetValue<long>()).UtcDateTime}");
        await ReplyAsync($"https://www.youtube.com/watch?v={video["videoId"]?.GetValue<string>()}");
    }

    private static char GetRandomLetter() => (char)new System.Random().Next('a', 'z' + 1);

    async private static Task<JsonNode> GetVideo()
    {
        JsonNode? video;
        while (true)
        {
            // invidious is a free of charge youtube frontend supporting http requests, see https://invidious.io/
            var jsonObject = await JSON.ParseJson($"https://y.com.sb/api/v1/search?q={GetRandomLetter()}{GetRandomLetter()}&fields=videoId,viewCount,published&date=today&page=20&pretty=1");
            video = jsonObject.Root.AsArray().LastOrDefault(x => !string.IsNullOrEmpty(x?["videoId"]?.GetValue<string>()));
            if (video != null) break;
        }

        return video;
    }
}
