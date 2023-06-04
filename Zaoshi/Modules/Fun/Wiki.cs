using Discord.Interactions;
using Genbox.Wikipedia;
using Genbox.Wikipedia.Enums;

namespace Zaoshi.Modules.Fun;

public class Wiki : InteractionModuleBase<SocketInteractionContext>
{
    public enum Language
    {
        RU,
        PT,
        ES,
        FR,
        DE,
        EN,
        CZ,
        SK
    }

    private readonly Dictionary<Language, WikiLanguage> _languages = new Dictionary<Language, WikiLanguage>{
        {Language.RU, WikiLanguage.Russian},
        {Language.PT, WikiLanguage.Portuguese},
        {Language.ES, WikiLanguage.Spanish},
        {Language.FR, WikiLanguage.French},
        {Language.DE, WikiLanguage.German},
        {Language.EN, WikiLanguage.English},
        {Language.CZ, WikiLanguage.Czech},
        {Language.SK, WikiLanguage.Slovak}
    };

    [SlashCommand("wiki", "Find an article on the Wikipedia")]
    public async Task Command(string query, Language language = Language.EN)
    {
        var client = new WikipediaClient();
        var req = new WikiSearchRequest(query){
            Limit = 1,
            WikiLanguage = _languages[language]
        };

        var response = (await client.SearchAsync(req)).QueryResult?.SearchResults.FirstOrDefault() ?? throw new Exception("Cannot find anything");

        await RespondAsync("Here is your Wikipedia article");
        await ReplyAsync($"{response.Url.ToString().Replace(" ", "%20")}");
    }
}
