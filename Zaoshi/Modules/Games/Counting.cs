using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Zaoshi.Attributes;
using Zaoshi.DB;
using static Zaoshi.DB.Collections;

#pragma warning disable CS1591

namespace Zaoshi.Modules.Games;

[Group("counting", "All the commands for the counting minigame")]
// ReSharper disable once ClassNeverInstantiated.Global
public class Counting : InteractionModuleBase<SocketInteractionContext>
{
    public enum Order
    {
        Ascending,
        Descending
    }

    [SlashCommand("set-channel", "Sets a counting channel, only one per server")]
    [RequireUserPermissions(new[]{GuildPermission.Administrator})]
    public async Task SetChannel(SocketTextChannel channel)
    {
        await DeferAsync();
        Cache.ServerSettings.UpdateOrAdd<ServerSettings>(channel.Guild.Id,
            nameof(ServerSettings.countingChannelId),
            channel.Id);
        var counting = Cache.Counting.GetOrFetch<Collections.Counting>(channel.Guild.Id);
        await Context.Guild.GetTextChannel(channel.Id).SendMessageAsync($"This is the new counting channel. Current number is {counting.count}, order is {(counting.isAscending ? "Ascending ⬆️" : "Descending ⬇️")}.");
        await FollowupAsync("Channel set");
    }

    [SlashCommand("set", "Sets a new number and order")]
    [RequireUserPermissions(new[]{GuildPermission.Administrator})]
    public async Task SetCount(int count, Order order = Order.Ascending)
    {
        await DeferAsync();
        Cache.Counting.UpdateOrAdd<Collections.Counting>(Context.Guild.Id,
            nameof(Collections.Counting.count),
            count);
        Cache.Counting.UpdateOrAdd<Collections.Counting>(Context.Guild.Id,
            nameof(Collections.Counting.isAscending),
            order == Order.Ascending);
        await FollowupAsync($"Count set to {count}, order is now {order.ToString()}.");
    }

    [SlashCommand("current", "Gets a current number in counting")]
    public async Task GetCount()
    {
        await DeferAsync();
        var counting = Cache.Counting.GetOrFetch<Collections.Counting>(Context.Guild.Id);
        var alreadyCounted = Context.Interaction.User.Id == counting.lastUserId;

        await FollowupAsync($"Current number is {counting.count}, order is {(counting.isAscending ? "Ascending ⬆️" : "Descending ⬇️")}.\n" +
                            $"{(alreadyCounted ? "You can count only once in a row, wait for someone else :)" : "You can count now")}.",
            ephemeral: true);
    }

    /// <summary>
    ///     Updates a count in cache
    /// </summary>
    /// <param name="serverId"></param>
    /// <param name="msg"></param>
    public static void OnCount(ulong serverId, SocketMessage msg)
    {
        var counting = Cache.Counting.GetOrFetch<Collections.Counting>(serverId);
        var addedNum = counting.isAscending ? 1 : -1;

        if (!int.TryParse(msg.Content, out var userCount) ||
            userCount != counting.count + addedNum ||
            msg.Author.Id == counting.lastUserId)
        {
            msg.DeleteAsync();
            return;
        }

        Cache.Counting.UpdateOrAdd<Collections.Counting>(serverId,
            new Dictionary<string, object>{
                {nameof(Collections.Counting.lastUserId), msg.Author.Id},
                {nameof(Collections.Counting.count), counting.count + addedNum}
            });

        msg.AddReactionAsync(Emoji.Parse("✅"));
    }
}
