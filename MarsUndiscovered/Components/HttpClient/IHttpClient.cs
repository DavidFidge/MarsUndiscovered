using System.Net.Http;
using System.Threading.Tasks;

namespace MarsUndiscovered.Components;

public interface IHttpClient : IDisposable
{
    Task<HttpResponseMessage> PostAsync(Uri uri, StringContent morgueJson);
}