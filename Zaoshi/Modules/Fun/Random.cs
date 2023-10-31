using Discord.Interactions;

#pragma warning disable CS1591

namespace Zaoshi.Modules.Fun;

public class Random : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("random", "Generates a random even number from <min, max> closed interval")]
    public async Task Command(int min, int max)
    {
        await RespondAsync(new System.Random().Next(min, max + 1).ToString());
    }
}
