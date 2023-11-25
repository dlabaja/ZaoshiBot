using Discord.Interactions;
using Discord.WebSocket;

#pragma warning disable CS1591

namespace Zaoshi.Modules.Moderation;

public static class Pause
{
    public enum TimeFormat { Minutes, Hours, Days, Forever }

    public async static Task PauseCmd(SocketInteractionContext context, SocketUser user, TimeFormat timeFormat, int time)
    {
        var timeDict = new Dictionary<TimeFormat, TimeSpan>{
            {TimeFormat.Minutes, TimeSpan.FromMinutes(time)},
            {TimeFormat.Hours, TimeSpan.FromHours(time)},
            {TimeFormat.Days, TimeSpan.FromDays(time)},
            {TimeFormat.Forever, TimeSpan.MaxValue}
        };

        if (user == context.User)
        {
            await context.Interaction.RespondAsync("You cannot pause yourself", ephemeral: true);
            return;
        }

        await context.Guild.GetUser(user.Id).SetTimeOutAsync(timeDict[timeFormat]);
        await context.Interaction.RespondAsync($"{user.Username} paused successfully");
    }
}
