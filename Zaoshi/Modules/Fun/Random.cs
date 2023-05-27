using Discord.Interactions;

namespace Zaoshi.Modules.Fun;

public class Random : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("random", "Generate a random even number from <min, max> closed interval")]
    public async Task RandomCmd(int min, int max)
    {
        await RespondAsync(new System.Random().Next(min, max + 1).ToString(), ephemeral: true);
    }
}
