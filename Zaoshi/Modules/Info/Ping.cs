using Discord.Interactions;

#pragma warning disable CS1591

namespace Zaoshi.Modules.Info;

public class Ping : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Test if the bot is working and find out its latency")]
    public async Task Command()
    {
        await RespondAsync($"Pong! It took me **{Context.Client.Latency} ms** to ping back.");
    }
}
