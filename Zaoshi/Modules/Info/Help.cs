using Discord;
using Discord.Interactions;
using System.Reflection;

namespace Zaoshi.Modules.Info;

public class Help : InteractionModuleBase<SocketInteractionContext>
{
    public enum CommandCategories
    {
        Fun,
        Info,
        Moderation
    }

    [SlashCommand("help", "Lists all available commands in certain category")]
    public async Task Command(CommandCategories category)
    {
        EmbedBuilder embed = new EmbedBuilder();
        var commands = Assembly.GetExecutingAssembly().GetExportedTypes()
            .Where(t => t.Namespace == $"Zaoshi.Modules.{category}");

        foreach (Type type in commands)
        {
            var methods = type.GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(SlashCommandAttribute), true).Length > 0);
            foreach (MethodInfo item in methods)
            {
                SlashCommandAttribute attribute = item.GetCustomAttribute<SlashCommandAttribute>()!;
                embed.AddField($"/{attribute.Name}", $"- {attribute.Description}");
            }
        }

        await RespondAsync(embed: embed.Build());
    }
}
