using Discord;
using Discord.Interactions;

#pragma warning disable CS1591

namespace Zaoshi.Modules.Info;

public class Credits : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("credits", "Shows creators of the bot")]
    public async Task Command()
    {
        var embed = new EmbedBuilder{
            Title = "The ones responsible for Zaoshi"
        };
        embed.AddField("@dlabaja", "Creation and programming of the bot", true);
        embed.Footer = new EmbedFooterBuilder().WithText("Thanks to all of you for contributing code and ideas to the project!");
        await RespondAsync(embed: embed.Build());
    }
}
