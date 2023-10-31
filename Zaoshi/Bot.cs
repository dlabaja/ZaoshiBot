using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

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
        AppDomain.CurrentDomain.UnhandledException += (_, args) => throw ((Exception)args.ExceptionObject);

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
        client.JoinedGuild += Events.OnGuildJoin;

        await _services.GetRequiredService<InteractionHandler>()
            .InitializeAsync();

        await client.LoginAsync(TokenType.Bot, IsDebug() ? Config.debugToken : Config.token);
        await client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private static Task LogAsync(LogMessage message)
    {
        Console.WriteLine(message.ToString());
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Debug token usage
    /// </summary>
    /// <returns>True if the bot currently uses debug token</returns>
    public static bool IsDebug()
    {
        #if DEBUG
        return true;
        #else
        return false;
        #endif
    }
}
