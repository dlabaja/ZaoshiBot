using Discord.Interactions;

#pragma warning disable CS1591

namespace Zaoshi.Modules.Fun;

public class Coin : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("coin", "Flip a coin to see what you get")]
    public async Task Command()
    {
        await RespondAsync(new System.Random().Next(2) == 0 ? "Head" : "Tails");
    }
}
