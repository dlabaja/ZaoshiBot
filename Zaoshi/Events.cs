using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Zaoshi.Modules;

namespace Zaoshi;

public static class Events
{
    public async static Task OnMessageReceived(SocketMessage arg)
    {
        if (new Random().Next(RandomReactions.reactionChance) == 0)
            await RandomReactions.PlaceReaction(arg);
    }
}
