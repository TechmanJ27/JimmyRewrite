public static async Task<string> FetchGiphyUrlAsync(string search)
{
    string url = $"https://giphy.com{GihpyKey}&q={Uri.EscapeDataString(search)}&limit=1&rating=pg13";

    HttpResponseMessage response = await client.GetAsync(url);
    response.EnsureSuccessStatusCode();

    string jsonResponse = await response.Content.ReadAsStringAsync();
    using JsonDocument doc = JsonDocument.Parse(jsonResponse);
    JsonElement root = doc.RootElement;

    JsonElement dataArray = root.GetProperty("data");
    
    if (dataArray.GetArrayLength() > 0)
    {
        JsonElement firstResult = dataArray[0];
        string gifUrl = firstResult
            .GetProperty("images")
            .GetProperty("original")
            .GetProperty("url")
            .GetString();

        return gifUrl;
    }

    throw new Exception("No GIFs found for the specified search term.");
}
