using System.Net.Http;

public void MakeRequest(string url)
{
    Information(url);
    try
    {
        using (var client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromMinutes(5);
            var response = client.GetAsync(url).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }
    }
    catch
    {
        Error(url);
        throw;
    }
}
