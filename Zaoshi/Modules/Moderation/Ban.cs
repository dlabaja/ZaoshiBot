using Discord.Interactions;
using Discord.WebSocket;
using Zaoshi.Attributes;
using static Discord.GuildPermission;

#pragma warning disable CS1591

namespace Zaoshi.Modules.Moderation;

public class Ban : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ban", "Bans a user from the server")]
    [RequireUserPermissions(new[]{Administrator, BanMembers})]
    public async Task Command(SocketUser user, int pruneDays = 0, string reason = "No reason given")
    {
        if (user == Context.User)
        {
            await RespondAsync("You cannot ban yourself");
            return;
        }

        await Context.Guild.AddBanAsync(user, pruneDays, reason);
        await RespondAsync($"{user.Username} banned successfully");
    }
}
