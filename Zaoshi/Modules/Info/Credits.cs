using Discord;
using Discord.Interactions;

namespace Zaoshi.Modules.Info;

public class Credits : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("credits", "Shows people who made the bot")]
    public async Task Command()
    {
        EmbedBuilder embed = new EmbedBuilder{
            Title = "The ones responsible for this"
        };
        embed.AddField("@Ten dlabaja#0369", "Creation and programming of the bot", true);
        embed.Footer = new EmbedFooterBuilder().WithText("Thanks to all of you for contributing code and ideas to the project!");
        await RespondAsync(embed: embed.Build());
    }
}
