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
        if (arg.Author.IsBot || arg.Author.IsWebhook) return;

        var serverId = (arg.Channel as SocketGuildChannel)!.Guild.Id;

        if (new Random().Next(RandomReactions.reactionChance) == 0)
            await RandomReactions.PlaceReaction(arg);

        var serverSettings = Cache.ServerSettings.GetOrFetch<Collections.ServerSettings>(serverId);
        if (arg.Channel.Id == serverSettings.countingChannelId)
        {
            Counting.OnCount(serverId, arg);
        }
        else if (arg.Channel.Id == serverSettings.wordFootballChannelId)
        {
            WordFootball.OnWord(serverId, arg);
        }
    }

    /// <summary>
    ///     Fires when the bot joins new server
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static Task OnGuildJoin(SocketGuild arg) => throw new NotImplementedException();
}
