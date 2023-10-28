using Discord.WebSocket;
using Zaoshi.DB;
using Zaoshi.Modules;
using Zaoshi.Modules.Games;

namespace Zaoshi;

/// <summary>
///     Discord API events
/// </summary>
public static class Events
{
    /// <summary>
    ///     Fires on a new message in guild
    /// </summary>
    /// <param name="arg"></param>
    public async static Task OnMessageReceived(SocketMessage arg)
    {
        // if (arg.Author.IsBot || arg.Author.IsWebhook) return;
        var serverId = (arg.Channel as SocketGuildChannel)!.Guild.Id;

        if (new Random().Next(RandomReactions.reactionChance) == 0)
            await RandomReactions.PlaceReaction(arg);
        
        if (arg.Channel.Id == (ulong)Cache.ServerSettings.GetOrFetch<Collections.ServerSettings>(serverId)[nameof(Collections.ServerSettings.CountingChannelId)].ToInt64())
        {
            Counting.OnCount(serverId, arg);
        }
    }

    public static Task OnNewGuild(SocketGuild arg) => throw new NotImplementedException();
}
