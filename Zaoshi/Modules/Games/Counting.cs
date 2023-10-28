using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Zaoshi.Attributes;
using Zaoshi.DB;
using static Zaoshi.DB.Collections;

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
            nameof(ServerSettings.CountingChannelId),
            channel.Id);

        var currentCount = Cache.Counting.GetOrFetch<Collections.Counting>(channel.Guild.Id)[nameof(Collections.Counting.Count)].ToInt32();

        await Context.Guild.GetTextChannel(channel.Id).SendMessageAsync($"This is the new counting channel. The first number is {currentCount}");
        await RespondAsync("Channel set");
    }

    [SlashCommand("set-count", "Sets a new count")]
    [RequireUserPermissions(new[]{GuildPermission.Administrator})]
    public async Task SetCount(int count, Order order = Order.Ascending)
    {
        Cache.Counting.UpdateOrAdd<Collections.Counting>(Context.Guild.Id,
            nameof(Collections.Counting.Count),
            count);
        Cache.Counting.UpdateOrAdd<Collections.Counting>(Context.Guild.Id,
            nameof(Collections.Counting.IsAscending),
            order == Order.Ascending);
        await RespondAsync($"Count set to {count}, order is now {order.ToString()}");
    }

    [SlashCommand("get-count", "Gets current count")]
    public async Task GetCount()
    {
        var isAscending = Cache.Counting.GetOrFetch<Collections.Counting>(Context.Guild.Id)[nameof(Collections.Counting.IsAscending)].ToBoolean();
        var currentCount = Cache.Counting.GetOrFetch<Collections.Counting>(Context.Guild.Id)[nameof(Collections.Counting.Count)].ToInt32();

        await RespondAsync($"Current count is {currentCount}, order is {(isAscending ? "Ascending" : "Descending")}", ephemeral: true);
    }

    /// <summary>
    ///     Updates a count in cache
    /// </summary>
    /// <param name="serverId"></param>
    /// <param name="msg"></param>
    public static void OnCount(ulong serverId, SocketMessage msg)
    {
        var isAscending = Cache.Counting.GetOrFetch<Collections.Counting>(serverId)[nameof(Collections.Counting.IsAscending)].ToBoolean();
        var currentCount = Cache.Counting.GetOrFetch<Collections.Counting>(serverId)[nameof(Collections.Counting.Count)].ToInt32();
        var addedNum = isAscending ? 1 : -1;

        if (!int.TryParse(msg.Content, out var userCount) || userCount != currentCount + addedNum)
        {
            msg.DeleteAsync();
            return;
        }

        msg.AddReactionAsync(Emoji.Parse("✅"));
        Cache.Counting.UpdateOrAdd<Collections.Counting>(serverId,
            nameof(Collections.Counting.Count),
            currentCount + addedNum);
    }
}
