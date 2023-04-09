using System.Net.Http;
using System.Threading.Tasks;

namespace MarsUndiscovered.Game.Components;

public class HttpClientWrapper : IHttpClient
{
    private HttpClient _httpClient;

    public HttpClientWrapper()
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }
    
    public void Dispose()
    {
        _httpClient?.Dispose();
    }

    public async Task<HttpResponseMessage> PostAsync(Uri uri, StringContent morgueJson)
    {
        return await _httpClient.PostAsync(uri, morgueJson);
    }
}