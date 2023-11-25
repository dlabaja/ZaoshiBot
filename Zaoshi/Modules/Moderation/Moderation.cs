using Discord.Interactions;
using Discord.WebSocket;
using Zaoshi.Attributes;
using static Discord.GuildPermission;

#pragma warning disable CS1591

namespace Zaoshi.Modules.Moderation;

[Group("moderation", "All the commands working with moderation")]
// ReSharper disable once ClassNeverInstantiated.Global
public class Moderation : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ban", "Bans a user from the server")]
    [RequireUserPermissions(new[]{Administrator, BanMembers})]
    public async Task Ban(SocketUser user, int pruneDays = 0, string reason = "No reason given")
        => await Modules.Moderation.Ban.BanCmd(Context, user, pruneDays, reason);

    [SlashCommand("kick", "Kicks a user from the server")]
    [RequireUserPermissions(new[]{Administrator, KickMembers})]
    public async Task Kick(SocketUser user, string reason = "No reason given")
        => await Modules.Moderation.Kick.KickCmd(Context, user, reason);

    [SlashCommand("pause", "Pauses (mutes) a user")]
    [RequireUserPermissions(new[]{Administrator, ModerateMembers})]
    public async Task Pause(SocketUser user, Pause.TimeFormat timeFormat, int time)
        => await Modules.Moderation.Pause.PauseCmd(Context, user, timeFormat, time);
}
