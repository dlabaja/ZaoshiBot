using Discord.Interactions;
using Zaoshi.Utils;

namespace Zaoshi.Modules.Fun;

public class YTSearch : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("yt-search", "Search a video on YouTube")]
    public async Task Command(string query)
    {
        // invidious is a free of charge youtube frontend supporting http requests, see https://invidious.io/
        await DeferAsync();
        var json = await JSON.ParseJson($"https://y.com.sb/api/v1/search?q={query}&fields=videoId&pretty=1");
        var video = json.Root[0];
        await FollowupAsync("Video:");
        await ReplyAsync($"https://www.youtube.com/watch?v={video?["videoId"]?.GetValue<string>()}");
    }
}
