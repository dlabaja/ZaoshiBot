using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;

namespace Zaoshi;

/// <summary>
///     Handles Discord interactions
/// </summary>
public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _handler;
    private readonly IServiceProvider _services;

    /// <summary>
    ///     Creates a new interaction handler
    /// </summary>
    /// <param name="client"></param>
    /// <param name="handler"></param>
    /// <param name="services"></param>
    public InteractionHandler(DiscordSocketClient client, InteractionService handler, IServiceProvider services)
    {
        _client = client;
        _handler = handler;
        _services = services;
    }

    /// <summary>
    ///     Initializes interaction handler
    /// </summary>
    public async Task InitializeAsync()
    {
        // Process when the client is ready, so we can register our commands.
        _client.Ready += ReadyAsync;
        _handler.Log += LogAsync;

        // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        // Process the InteractionCreated payloads to execute Interactions commands
        _client.InteractionCreated += HandleInteraction;
    }

    private static Task LogAsync(LogMessage log) =>
        Task.CompletedTask;

    async private Task ReadyAsync()
    {
        // Context & Slash commands can be automatically registered, but this process needs to happen after the client enters the READY state.
        // Since Global Commands take around 1 hour to register, we should use a test guild to instantly update and test our commands.
        if (Bot.IsDebug())
            foreach (var guildId in Config.testGuilds)
            {
                await _handler.RegisterCommandsToGuildAsync(guildId);
            }
        else
            await _handler.RegisterCommandsGloballyAsync();
    }

    async private Task HandleInteraction(SocketInteraction interaction)
    {
        var context = new SocketInteractionContext(_client, interaction);
        await _handler.ExecuteCommandAsync(context, _services);
    }
}
