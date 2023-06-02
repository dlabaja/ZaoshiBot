using Discord.Interactions;

namespace Zaoshi;

public class Ping : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Test if the bot is working and find out it's latency")]
    public async Task Command()
    {
        await RespondAsync($"Pong! It took **{Context.Client.Latency} ms** to get to me.");
    }
}
