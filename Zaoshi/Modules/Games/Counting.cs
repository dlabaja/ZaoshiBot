using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Zaoshi.Attributes;
using Zaoshi.DB;
using static Zaoshi.DB.Collections;

#pragma warning disable CS1591

namespace Zaoshi.Modules.Games;

[Group("counting", "All the commands for the counting minigame")]
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
        Cache.ServerSettings.UpdateOrAdd<ServerSettings>(channel.Guild.Id,
            nameof(ServerSettings.countingChannelId),
            channel.Id);

        var currentCount = Cache.Counting.GetOrFetch<Collections.Counting>(channel.Guild.Id)[nameof(Collections.Counting.count)].ToInt32();
        var isAscending = Cache.Counting.GetOrFetch<Collections.Counting>(Context.Guild.Id)[nameof(Collections.Counting.isAscending)].ToBoolean();

        await Context.Guild.GetTextChannel(channel.Id).SendMessageAsync($"This is the new counting channel. Current number is {currentCount}, order is {(isAscending ? "Ascending ⬆️" : "Descending ⬇️")}");
        await RespondAsync("Channel set");
    }

    [SlashCommand("set", "Sets a new number and order")]
    [RequireUserPermissions(new[]{GuildPermission.Administrator})]
    public async Task SetCount(int count, Order order = Order.Ascending)
    {
        Cache.Counting.UpdateOrAdd<Collections.Counting>(Context.Guild.Id,
            nameof(Collections.Counting.count),
            count);
        Cache.Counting.UpdateOrAdd<Collections.Counting>(Context.Guild.Id,
            nameof(Collections.Counting.isAscending),
            order == Order.Ascending);
        await RespondAsync($"Count set to {count}, order is now {order.ToString()}");
    }

    [SlashCommand("current", "Gets a current number in counting")]
    public async Task GetCount()
    {
        var isAscending = Cache.Counting.GetOrFetch<Collections.Counting>(Context.Guild.Id)[nameof(Collections.Counting.isAscending)].ToBoolean();
        var currentCount = Cache.Counting.GetOrFetch<Collections.Counting>(Context.Guild.Id)[nameof(Collections.Counting.count)].ToInt32();
        var alreadyCounted = Context.Interaction.User.Id == (ulong)Cache.Counting.GetOrFetch<Collections.Counting>(Context.Guild.Id)[nameof(Collections.Counting.lastUserId)].AsInt64;

        await RespondAsync($"Current number is {currentCount}, order is {(isAscending ? "Ascending ⬆️" : "Descending ⬇️")}\n" +
                           $"{(alreadyCounted ? "You can count only once in a row, wait for someone else :)" : "You can count now")}",
            ephemeral: true);
    }

    /// <summary>
    ///     Updates a count in cache
    /// </summary>
    /// <param name="serverId"></param>
    /// <param name="msg"></param>
    public static void OnCount(ulong serverId, SocketMessage msg)
    {
        var isAscending = Cache.Counting.GetOrFetch<Collections.Counting>(serverId).GetValue(nameof(Collections.Counting.isAscending)).ToBoolean();
        var currentCount = Cache.Counting.GetOrFetch<Collections.Counting>(serverId)[nameof(Collections.Counting.count)].ToInt32();
        var addedNum = isAscending ? 1 : -1;

        if (!int.TryParse(msg.Content, out var userCount) ||
            userCount != currentCount + addedNum ||
            msg.Author.Id == (ulong)Cache.Counting.GetOrFetch<Collections.Counting>(serverId)[nameof(Collections.Counting.lastUserId)].AsInt64)
        {
            msg.DeleteAsync();
            return;
        }

        Cache.Counting.UpdateOrAdd<Collections.Counting>(serverId, nameof(Collections.Counting.lastUserId), msg.Author.Id);

        msg.AddReactionAsync(Emoji.Parse("✅"));
        Cache.Counting.UpdateOrAdd<Collections.Counting>(serverId,
            nameof(Collections.Counting.count),
            currentCount + addedNum);
    }
}
