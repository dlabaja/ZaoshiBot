using Discord.Interactions;
using Discord.WebSocket;

#pragma warning disable CS1591

namespace Zaoshi.Modules.Moderation;

public static class Ban
{
    public async static Task BanCmd(SocketInteractionContext context, SocketUser user, int pruneDays = 0, string reason = "No reason given")
    {
        if (user == context.User)
        {
            await context.Interaction.RespondAsync("You cannot ban yourself");
            return;
        }

        await context.Guild.AddBanAsync(user, pruneDays, reason);
        await context.Interaction.RespondAsync($"{user.Username} banned successfully");
    }
}
