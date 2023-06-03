using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Zaoshi.Attributes;

namespace Zaoshi.Modules.Moderation;

public class Ban : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ban", "Bans a user from the server")]
    [RequireUserPermissions(new[]{GuildPermission.Administrator, GuildPermission.BanMembers})]
    public async Task Command(SocketUser user, int pruneDays = 0, string reason = "No reason given")
    {
        if (user == Context.User) throw new Exception("You cannot ban yourself");
        await Context.Guild.AddBanAsync(user, pruneDays, reason);
        await RespondAsync($"{user.Username} banned successfully");
    }
}
