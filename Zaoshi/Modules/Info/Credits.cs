using Discord;
using Discord.Interactions;
using System.Threading.Tasks;

namespace Zaoshi.Modules.Info;

public class Credits : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("credits", "Shows people who made the bot")]
    public async Task CreditsCmd()
    {
        EmbedBuilder embed = new EmbedBuilder{
            Title = "The ones responsible for this"
        };
        embed.AddField(new EmbedFieldBuilder().WithName("@Ten dlabaja#0369").WithValue("Creation and programming of the bot"));
        embed.Footer = new EmbedFooterBuilder().WithText("Thanks to all of you for contributing code and ideas to the project!");
        await RespondAsync(embed: embed.Build());
    }
}
