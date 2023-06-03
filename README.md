# ZaoshiBot

Modern and open source Discord bot written in C#.

To run it yourself, create config.json file next to the executable and put this into it (or just run one of the releases):

```json
{
  "token": "bot token",
  "debugToken": "token for debug version of the bot, not necesary",
  "testGuilds": ["uids of guilds you want to debug in (int64), can be empty"]
}
```
