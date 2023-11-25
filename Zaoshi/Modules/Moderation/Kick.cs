using Discord.Interactions;
using Discord.WebSocket;

#pragma warning disable CS1591

namespace Zaoshi.Modules.Moderation;

public static class Kick
{
    public async static Task KickCmd(SocketInteractionContext context, SocketUser user, string reason = "No reason given")
    {
        if (user == context.User)
        {
            await context.Interaction.RespondAsync("You cannot kick yourself");
            return;
        }

        await context.Guild.GetUser(user.Id).KickAsync(reason);
        await context.Interaction.RespondAsync($"{user.Username} kicked successfully");
    }
}
