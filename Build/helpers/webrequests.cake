using System.Net.Http;

public static void MakeRequest(this ICakeContext context, string url)
{
    context.Information(url);
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
        context.Error(url);
        throw;
    }
}
