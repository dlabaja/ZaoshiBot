using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Zaoshi.Utils;

namespace Zaoshi.Modules.Moderation;

public class Pause : InteractionModuleBase<SocketInteractionContext>
{
    public enum TimeFormat { Minutes, Hours, Days, Forever }

    [SlashCommand("pause", "Pauses (mutes) a user from the server")]
    [RequireUserPermissions(new[]{GuildPermission.Administrator, GuildPermission.KickMembers, GuildPermission.BanMembers, GuildPermission.ModerateMembers})]
    public async Task Command(SocketUser user, TimeFormat timeFormat, int time)
    {
        var timeDict = new Dictionary<TimeFormat, TimeSpan>{
            {TimeFormat.Minutes, TimeSpan.FromMinutes(time)},
            {TimeFormat.Hours, TimeSpan.FromHours(time)},
            {TimeFormat.Days, TimeSpan.FromDays(time)},
            {TimeFormat.Forever, TimeSpan.MaxValue}
        };

        if (user == Context.User) throw new Exception("You cannot pause yourself");
        await Context.Guild.GetUser(user.Id).SetTimeOutAsync(timeDict[timeFormat]);
        await RespondAsync($"{user.Username} paused successfully");
    }
}
