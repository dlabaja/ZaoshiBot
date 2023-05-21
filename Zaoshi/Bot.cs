using Discord;
using Discord.WebSocket;

namespace Zaoshi;

internal class Bot
{
    private readonly DiscordSocketClient _client;

    private Bot()
    {
        DiscordSocketConfig config = new DiscordSocketConfig{
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };
        _client = new DiscordSocketClient(config);
        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.MessageReceived += MessageReceivedAsync;
        _client.InteractionCreated += InteractionCreatedAsync;
    }

    private static void Main()
        => new Bot()
            .MainAsync()
            .GetAwaiter()
            .GetResult();

    async private Task MainAsync()
    {
        await _client.LoginAsync(TokenType.Bot, Secrets.token);
        await _client.StartAsync();
        await Task.Delay(Timeout.Infinite);
    }

    private static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }

    private Task ReadyAsync()
    {
        Console.WriteLine($"{_client.CurrentUser} is connected!");
        return Task.CompletedTask;
    }

    // This is not the recommended way to write a bot - consider
    // reading over the Commands Framework sample.
    async private Task MessageReceivedAsync(SocketMessage message)
    {
        // The bot should never respond to itself.
        if (message.Author.Id == _client.CurrentUser.Id)
            return;

        if (message.Content == "!ping")
        {
            // Create a new ComponentBuilder, in which dropdowns & buttons can be created.
            ComponentBuilder? cb = new ComponentBuilder()
                .WithButton("Click me!", "unique-id");

            // Send a message with content 'pong', including a button.
            // This button needs to be build by calling .Build() before being passed into the call.
            await message.Channel.SendMessageAsync("pong!", components: cb.Build());
        }
    }

    // For better functionality & a more developer-friendly approach to handling any kind of interaction, refer to:
    // https://discordnet.dev/guides/int_framework/intro.html
    async private Task InteractionCreatedAsync(SocketInteraction interaction)
    {
        // safety-casting is the best way to prevent something being cast from being null.
        // If this check does not pass, it could not be cast to said type.
        if (interaction is SocketMessageComponent component)
        {
            // Check for the ID created in the button mentioned above.
            if (component.Data.CustomId == "unique-id")
                await interaction.RespondAsync("Thank you for clicking my button!");

            else
                Console.WriteLine("An ID has been received that has no handler!");
        }
    }
}
