using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Zaoshi;

public class Bot
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _services;

    private readonly DiscordSocketConfig _socketConfig = new DiscordSocketConfig{
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
        AlwaysDownloadUsers = true
    };

    private Bot()
    {
        Console.WriteLine($"Debug mode: {IsDebug()}");
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("config.json")
            .Build();
        _services = new ServiceCollection()
            .AddSingleton(_socketConfig)
            .AddSingleton(_configuration)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .BuildServiceProvider();
    }

    private static void Main(string[] args)
        => new Bot().RunAsync()
            .GetAwaiter()
            .GetResult();

    async private Task RunAsync()
    {
        DiscordSocketClient client = _services.GetRequiredService<DiscordSocketClient>();

        client.Log += LogAsync;
        client.MessageReceived += Events.OnMessageReceived;

        // Here we can initialize the service that will register and execute our commands
        await _services.GetRequiredService<InteractionHandler>()
            .InitializeAsync();

        // Bot token can be provided from the Configuration object we set up earlier
        await client.LoginAsync(TokenType.Bot, IsDebug() ? _configuration["debugToken"] : _configuration["token"]);
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
