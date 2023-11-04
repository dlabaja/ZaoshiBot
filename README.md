# ZaoshiBot

Modern and open source Discord bot written in C#.

To run it yourself, create config.json file next to the executable and put this into it

```json
{
  "token": "bot token",
  "debugToken": "token for debug version of the bot (optional)",
  "testGuilds": ["uids of guilds you want to debug in (int64) (optional)"],
  "connectionString": "connection string for your MongoDB database, visit https://www.mongodb.com for more info"
}
```

# Capabilities
Fun commands
- Generate random YT video
- Search YT videos on the fly
- Search Wikipedia
- Calculate any expression
- Throw a coin, generate a random number, etc.

Minigames
- Counting
- Word football

Moderation
- Ban, Kick, Pause

Supports Discord Interactions\
Pretty fast thanks to MongoDB and caching system in place\
Fully open source and editable (you can see everything except my token key :))
