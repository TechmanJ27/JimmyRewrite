using System.Net.Http.Headers
using System.Threading.Tasks

public class KlipyResponse
{
    public KlipyData Data { get; set; }
}

public class KlipyData
{
    public KlipyGif[] Results { get; set; }
}

public class KlipyGif
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
}

public class GifService(IConfiguration config)
{
    
    private static readonly HttpClient client = new HttpClient();
    
    public async Task FetchGIF(string query, User user) {
    
        string apiKey = config["Klipy:ApiKey"]; 
        string customerId = user.id;
        string page = "1";
        string perPage = "10";
        string countryCode = "en";
        string contentFilter = "on";
        
        string url = $"https://api.klipy.com/api/v1/{apiKey}/gifs/search?page={page}&per_page={perPage}&q={Uri.EscapeDataString(query)}&customer_id={customerId}&locale={countryCode}&content_filter={contentFilter}";
        
        var request = new HttpRequestMessage(HttpMethod.Get, url);
    
        var response = await client.SendAsync(request);    
        response.EnsureSuccessStatusCode();
        string jsonString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        KlipyResponse result = JsonSerializer.Deserialize<KlipyResponse>(jsonString, options);
    
        return result;
    }
}
