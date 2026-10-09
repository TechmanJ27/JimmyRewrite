using NetCord.Services.ApplicationCommands;

namespace JimmyRewrite.Commands;

public partial class CommandsModule
{
    private enum CoinSides
    {
        Heads = 1,
        Tails = 2
    }
    
    [SlashCommand("rand", "Roll a dice!... or coin")]
    public static string Rand(
        [SlashCommandParameter(Name = "sides", Description = "How many sides would you like on your dice?... Two sides for a coin flip", MinValue = 2, MaxValue = 100)]
        int sides = 6,
        [SlashCommandParameter(Name = "rolls", Description = "How many times do you wanna roll/flip it?", MinValue = 1, MaxValue = 100)]
        int rolls = 1
        )
    {
        var results = new int[rolls];
        for (var i = 0; i < rolls; i++)
        {
            results[i] = Random.Shared.Next(1, sides + 1);
        }
        
        var min = results.Min();
        var max = results.Max();
        var avg = (int)results.Average();
        var sum = results.Sum();

        if (sides == 2)
        {
            if (rolls <= 1) return $":coin: You got ${IsHeadOrTails(results[0])}!";
            
            var heads = results.Count(x => x == (int)CoinSides.Heads);
            var tails = results.Count(x => x == (int)CoinSides.Tails);
            return $"> **{string.Join(", ", IsHeadOrTailsShort(results))}**\n" +
                   $":coin: total: **{sum}** | heads: **{heads}** | tails: **{tails}**";
        }

        if (rolls <= 1) return $":game_die: You rolled a **{results[0]}**!";
        
        return $"> **{string.Join(", ", results)}**\n" +
               $":game_die: total: **{sum}** | min: **{min}** | max: **{max}** | avg: **{avg}** | **{rolls}d{sides}**";

        string IsHeadOrTails(int value) => value == (int)CoinSides.Heads ? "Head" : "Tails";

        string[] IsHeadOrTailsShort(int[] values)
        {
            var returnValue = new string[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                returnValue[i] = values[i] == (int)CoinSides.Heads ? "H" : "T";
            }

            return returnValue;
        }
    }

    [SlashCommand("gif", "Get a random gif from the chosen category")]
    public partial class GIFModule : ApplicationCommandModule<ApplicationCommandContext>
    {
    [SubSlashCommand("cat", "Get a random cat gif")]
        public static string Cat() {
            HttpClient = new HttpClient();

            HttpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://beta-api.thecatapi.com/v1/images/search");

            string cat_key = config["GIFS:cat_key"];
            request.Headers.Add("x-api-key", cat_key);

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody.url;
        }
    }

    public partial class GIFModule : ApplicationCommandModule<ApplicationCommandContext> 
    {
    [SubSlashCommand("dog", "Get a random dog gif")]
        public static string Dog() {
            HttpClient = new HttpClient();
            
            HttpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://dog.ceo/api/breeds/image/random");

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody.message;
        }
    }

    public partial class GIFModule : ApplicationCommandModule<ApplicationCommandContext> 
    {
    [SubSlashCommand("boykisser", "Get a random boykisser gif")]
        public static string Boykisser() {
            
        }
    }
}
