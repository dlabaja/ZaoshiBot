using Discord;
using Discord.Interactions;

#pragma warning disable CS1591

namespace Zaoshi.Modules.Info;

public class ServerInfo : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("server-info", "Shows info about the server")]
    public async Task Command()
    {
        await DeferAsync();
        await FollowupAsync(embed: new EmbedBuilder()
            .WithTitle("Server info")
            .WithImageUrl(Context.Guild.IconUrl)
            .WithThumbnailUrl(Context.Guild.BannerUrl)
            .AddField("ID", Context.Guild.Id, true)
            .AddField("Name", Context.Guild.Name, true)
            .AddField("Description", Context.Guild.Description ?? "None", true)
            .AddField("Created at", Context.Guild.CreatedAt.ToString("dd.MM.yyyy"), true)
            .AddField("Owner", Context.Guild.Owner.Mention, true)
            .AddField("Users", Context.Guild.MemberCount, true)
            .AddField("Channels", Context.Guild.Channels.Count, true)
            .AddField("Roles", Context.Guild.Roles.Count, true)
            .AddField("Emotes", Context.Guild.Emotes.Count, true)
            .AddField("Sticker", Context.Guild.Stickers.Count, true)
            .Build());
    }
}
