using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;

namespace Zaoshi;

public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _handler;
    private readonly IServiceProvider _services;

    public InteractionHandler(DiscordSocketClient client, InteractionService handler, IServiceProvider services)
    {
        _client = client;
        _handler = handler;
        _services = services;
    }

    public async Task InitializeAsync()
    {
        // Process when the client is ready, so we can register our commands.
        _client.Ready += ReadyAsync;
        _handler.Log += LogAsync;

        // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        // Process the InteractionCreated payloads to execute Interactions commands
        _client.InteractionCreated += HandleInteraction;
        _handler.InteractionExecuted += HandleInteractionExecuted;
    }

    // Returns any error to the user
    async private static Task HandleInteractionExecuted(ICommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        if (result.ErrorReason == null) return;
        var errorAliases = new Dictionary<string, string>{
            {"The server responded with error 50013: Missing Permissions", "Missing Permissions"},
            {"Offset cannot be more than 28 days from the current date. (Parameter 'span')", "Maximum pause time is 28 days"}
        };

        await context.Interaction.RespondAsync(errorAliases.TryGetValue(result.ErrorReason, out var error) ? error : result.ErrorReason, ephemeral: true);
    }

    private static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log);
        return Task.CompletedTask;
    }

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
