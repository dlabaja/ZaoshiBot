using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Zaoshi.Exceptions;

namespace Zaoshi;

/// <summary>
///     Main class of the bot
/// </summary>
public class Bot
{
    private readonly IServiceProvider _services;

    private readonly DiscordSocketConfig _socketConfig = new DiscordSocketConfig{
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers | GatewayIntents.MessageContent,
        AlwaysDownloadUsers = true
    };

    private Bot()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) => throw new FatalException((Exception)args.ExceptionObject);

        Console.WriteLine($"Debug mode: {IsDebug()}");
        _services = new ServiceCollection()
            .AddSingleton(_socketConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .BuildServiceProvider();
    }

    private static void Main()
        => new Bot().RunAsync()
            .GetAwaiter()
            .GetResult();

    async private Task RunAsync()
    {
        var client = _services.GetRequiredService<DiscordSocketClient>();

        client.Log += LogAsync;
        client.MessageReceived += Events.OnMessageReceived;
        client.JoinedGuild += Events.OnNewGuild;

        // Here we can initialize the service that will register and execute our commands
        await _services.GetRequiredService<InteractionHandler>()
            .InitializeAsync();

        // Bot token can be provided from the Configuration object we set up earlier
        await client.LoginAsync(TokenType.Bot, IsDebug() ? Config.GetDebugToken() : Config.GetToken());
        await client.StartAsync();

        // Never quit the program until manually forced to.
        await Task.Delay(Timeout.Infinite);
    }

    private static Task LogAsync(LogMessage message)
    {
        Console.WriteLine(message.ToString());
        return Task.CompletedTask;
    }

    public static bool IsDebug()
    {
        #if DEBUG
        return true;
        #else
        return false;
        #endif
    }
}
