using Microsoft.Extensions.DependencyInjection;

namespace MarsUndiscovered.Tests;

public static class ServiceProviderExtensions
{
    public static T Resolve<T>(this IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<T>();
    }
}
