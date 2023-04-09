using System.Net.Http;
using System.Threading.Tasks;

namespace MarsUndiscovered.Game.Components;

public interface IHttpClient : IDisposable
{
    Task<HttpResponseMessage> PostAsync(Uri uri, StringContent morgueJson);
}