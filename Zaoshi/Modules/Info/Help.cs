using Discord;
using Discord.Interactions;
using System.Reflection;

#pragma warning disable CS1591

namespace Zaoshi.Modules.Info;

public class Help : InteractionModuleBase<SocketInteractionContext>
{
    public enum CommandCategories
    {
        Fun,
        Info,
        Moderation,
        Games
    }

    [SlashCommand("help", "Lists all available commands in certain category")]
    public async Task Command(CommandCategories category)
    {
        var embed = new EmbedBuilder();
        var commands = Assembly.GetExecutingAssembly().GetExportedTypes()
            .Where(t => t.Namespace == $"Zaoshi.Modules.{category}");

        foreach (var type in commands)
        {
            var methods = type.GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(SlashCommandAttribute), true).Length > 0);
            foreach (var item in methods)
            {
                var attribute = item.GetCustomAttribute<SlashCommandAttribute>()!;
                var groupAttribute = item.DeclaringType?.GetCustomAttribute<GroupAttribute>();
                if (groupAttribute != null)
                {
                    embed.AddField($"/{groupAttribute.Name} {attribute.Name}", $"- {attribute.Description}");
                    continue;
                }

                embed.AddField($"/{attribute.Name}", $"- {attribute.Description}");
            }
        }

        await RespondAsync(embed: embed.Build());
    }
}
