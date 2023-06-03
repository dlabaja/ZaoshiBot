using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Zaoshi.Attributes;

namespace Zaoshi.Modules.Moderation;

public class Kick : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("kick", "Kicks a user from the server")]
    [RequireUserPermissions(new[]{GuildPermission.Administrator, GuildPermission.KickMembers, GuildPermission.BanMembers})]
    public async Task Command(SocketUser user, string reason = "No reason given")
    {
        if (user == Context.User) throw new Exception("You cannot kick yourself");
        await Context.Guild.GetUser(user.Id).KickAsync(reason);
        await RespondAsync($"{user.Username} kicked successfully");
    }
}
