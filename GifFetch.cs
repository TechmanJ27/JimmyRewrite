using System.Net.Http.Headers
using System.Threading.Tasks

Task fetchGIF(String query) {
  var client = new HttpClient();
  var request = new HttpRequestMessage(HttpMethod.Get, "https://api.klipy.com/api/v1/{app_key}/gifs/search?page={page}&per_page={per_page}&q={q}&customer_id={customer_id}&locale={country_code}&content_filter={content_filter}");
  var content = new StringContent(string.Empty);
  content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
  request.Content = content;
  var response = await client.SendAsync(request);
  response.EnsureSuccessStatusCode();
  const jsonString = await response.Content.ReadAsStringAsync());
   = JsonSerializer.Deserialize<User>(jsonString);
   
}
