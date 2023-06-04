using Discord.Interactions;
using Discord.WebSocket;

namespace Zaoshi.Modules.Games;

[Group("counting", "All the commands for the counting minigame")]
public class Counting : InteractionModuleBase<SocketInteractionContext>
{
    public enum Order
    {
        Ascending,
        Descending
    }

    [SlashCommand("set-channel", "Sets a counting channel, only one per server")]
    public async Task SetChannel(SocketTextChannel channel)
    {
        //pokud není v databázi nastav i count
        await RespondAsync();
    }

    [SlashCommand("set-count", "Sets new count")]
    public async Task SetCount(int count, Order order = Order.Ascending)
    {
        await RespondAsync();
    }
}
