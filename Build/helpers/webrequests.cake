using System.Net;
using System.Net.Http;

public void MakeRequest(string url)
{
    Information($"Requesting URL {url}");
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
    catch (Exception ex)
    {
        Error($"{ex.ToString()}");
        throw;
    }
}

public bool GetServerFileInfo(string url, out DateTime modified, out long size)
{
    modified = default;
    size = default;

    Information($"Getting file info by the URL {url}");
    try
    {
        using (var client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromMinutes(5);
            var response = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            modified = response.Content.Headers.LastModified.Value.DateTime;
            size = response?.Content?.Headers?.ContentLength is null ? 0 : (long)response.Content.Headers.ContentLength;
            if (modified == default)
            {
                throw new ArgumentNullException(nameof(modified));
            }
            if (size <= 0)
            {
                throw new ArgumentNullException(nameof(size));
            }
        }
        Information("Head request successfully completed");
        return true;
    }
    catch (Exception ex)
    {
        Error($"{ex.ToString()}");
    }
    return false;
}