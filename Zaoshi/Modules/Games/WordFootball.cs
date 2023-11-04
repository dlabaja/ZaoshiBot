using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using Zaoshi.Attributes;
using Zaoshi.DB;
using static Zaoshi.DB.Collections;

#pragma warning disable CS1591

namespace Zaoshi.Modules.Games;

[Group("word-football", "All the commands for the word football minigame")]
// ReSharper disable once ClassNeverInstantiated.Global
public class WordFootball : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("set-channel", "Sets a word football channel, only one per server")]
    [RequireUserPermissions(new[]{GuildPermission.Administrator})]
    public async Task SetChannel(SocketTextChannel channel)
    {
        await DeferAsync();
        Cache.ServerSettings.UpdateOrAdd<ServerSettings>(channel.Guild.Id,
            nameof(ServerSettings.wordFootballChannelId),
            channel.Id);
        var football = Cache.WordFootball.GetOrFetch<Collections.WordFootball>(channel.Guild.Id);
        await Context.Guild.GetTextChannel(channel.Id).SendMessageAsync($"This is the new word football channel. Current letter is '{football.lastLetter}'.");
        await FollowupAsync("Channel set");
    }

    [SlashCommand("set", "Sets a new letter")]
    [RequireUserPermissions(new[]{GuildPermission.Administrator})]
    public async Task SetLetter(char letter)
    {
        await DeferAsync();
        Cache.WordFootball.UpdateOrAdd<Collections.WordFootball>(Context.Guild.Id,
            nameof(Collections.WordFootball.lastLetter),
            letter);
        await FollowupAsync($"Letter set to '{letter}'.");
    }

    [SlashCommand("current", "Gets a current number in counting")]
    public async Task GetLetter()
    {
        await DeferAsync();
        var football = Cache.WordFootball.GetOrFetch<Collections.WordFootball>(Context.Guild.Id);
        var alreadyPlayed = Context.Interaction.User.Id == football.lastUserId;

        await FollowupAsync($"Current letter is '{(football.lastLetter == default ? "not set yet, be the first to kick it" : football.lastLetter)}'.\n" +
                            $"{(alreadyPlayed ? "You can play only once in a row, wait for someone else :)" : "You can play now")}.",
            ephemeral: true);
    }

    /// <summary>
    ///     Updates a count in cache
    /// </summary>
    /// <param name="serverId"></param>
    /// <param name="msg"></param>
    public static void OnWord(ulong serverId, SocketMessage msg)
    {
        var football = Cache.WordFootball.GetOrFetch<Collections.WordFootball>(serverId);
        var word = Regex.Replace(msg.Content, @"[^\p{L}]+", "");

        if (string.IsNullOrEmpty(word))
            return;

        if ((word[^1] != football.lastLetter || msg.Author.Id == football.lastUserId) && football.lastLetter != default)
        {
            msg.DeleteAsync();
            return;
        }

        Cache.WordFootball.UpdateOrAdd<Collections.WordFootball>(serverId,
            new Dictionary<string, object>{
                {nameof(Collections.WordFootball.lastUserId), msg.Author.Id},
                {nameof(Collections.WordFootball.lastLetter), word[^1]}
            });

        msg.AddReactionAsync(Emoji.Parse("✅"));
    }
}
