using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Zaoshi.Modules;

public static class RandomReactions
{
    public const int reactionChance = 20;

    public async static Task PlaceReaction(SocketMessage msg)
    {
        if (msg.Channel is SocketGuildChannel guildChannel)
        {
            var emotes = guildChannel.Guild.Emotes;
            await msg.AddReactionAsync(emotes.ToList()[new Random().Next(emotes.Count)]);
        }
    }
}
