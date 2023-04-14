using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace MarsUndiscovered.Game.Components;

public class MorgueWebService : BaseComponent, IMorgueWebService
{
    private readonly string _morgueBaseAddress;
    private readonly string _morguePort;
    private readonly string _morgueEndPoint;
    private readonly Uri _uri;
    private IHttpClient _httpClient;

    public MorgueWebService(IHttpClient httpClient, IConfiguration configuration = null, ILogger logger = null)
    {
        // Property injection only happens after constructor but we need the logger in the constructor
        if (Logger == null)
            Logger = logger;
        
        _httpClient = httpClient;
        
        _morgueBaseAddress = configuration.GetValueOrEnvironmentVariable("SENDMORGUE_BASEADDRESS");
        _morguePort = configuration.GetValueOrEnvironmentVariable("SENDMORGUE_PORT");
        _morgueEndPoint = configuration.GetValueOrEnvironmentVariable("SENDMORGUE_ENDPOINT");

        if (String.IsNullOrEmpty(_morgueBaseAddress))
        {
            Logger?.Warning("SENDMORGUE_BASEADDRESS is not set. Cannot send morgue files to web site.");
            return;
        }
        
        if (!String.IsNullOrEmpty(_morguePort) && !int.TryParse(_morguePort, out _))
        {
            Logger?.Warning($"SENDMORGUE_PORT {_morguePort} is not a number. Defaulting to no port in URL.");
        }

        if (String.IsNullOrEmpty(_morgueEndPoint))
        {
            Logger?.Warning("SENDMORGUE_ENDPOINT is not set. Cannot send morgue files to web site.");
            return;
        }
        
        var uri = new UriBuilder();

        uri.Scheme = Uri.UriSchemeHttps;
        uri.Host = _morgueBaseAddress;
        
        if (!String.IsNullOrEmpty(_morguePort) && int.TryParse(_morguePort, out _))
            uri.Port = int.Parse(_morguePort);
        
        uri.Path = _morgueEndPoint;
        
        _uri = uri.Uri;
    }
    
    public async Task SendMorgue(MorgueExportData morgueExportData)
    {
        if (_morgueBaseAddress == null)
            return;
        
        Logger?.Information($"Sending morgue file to {_uri}");
        
        using StringContent morgueJson = new(JsonConvert.SerializeObject(morgueExportData),
            Encoding.UTF8,
            "application/json");

        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Method = HttpMethod.Post;

        var response = await _httpClient.PostAsync(_uri, morgueJson);

        if (response.EnsureSuccessStatusCode().IsSuccessStatusCode) // Throws exception if not successful
            Logger?.Information($"Successfully sent morgue file to {_uri}");
    }
}