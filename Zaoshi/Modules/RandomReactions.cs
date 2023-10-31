using Discord.Net;
using Discord.WebSocket;

namespace Zaoshi.Modules;

/// <summary>
///     Reacts with a random server emote to a message
/// </summary>
public static class RandomReactions
{
    /// <summary>
    ///     Chance of placing a reaction
    /// </summary>
    public const int reactionChance = 20;

    /// <summary>
    ///     Places a reaction
    /// </summary>
    /// <param name="msg"></param>
    public async static Task PlaceReaction(SocketMessage msg)
    {
        if (msg.Channel is SocketGuildChannel guildChannel)
        {
            var emotes = guildChannel.Guild.Emotes;
            try
            {
                await msg.AddReactionAsync(emotes.ToList()[new Random().Next(emotes.Count)]);
            }
            catch (HttpException) {}
        }
    }
}
