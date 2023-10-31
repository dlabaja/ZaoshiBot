using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Zaoshi.Attributes;

#pragma warning disable CS1591

namespace Zaoshi.Modules.Moderation;

public class Kick : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("kick", "Kicks a user from the server")]
    [RequireUserPermissions(new[]{GuildPermission.Administrator, GuildPermission.KickMembers, GuildPermission.BanMembers})]
    public async Task Command(SocketUser user, string reason = "No reason given")
    {
        if (user == Context.User)
        {
            await RespondAsync("You cannot kick yourself");
            return;
        }

        await Context.Guild.GetUser(user.Id).KickAsync(reason);
        await RespondAsync($"{user.Username} kicked successfully");
    }
}
