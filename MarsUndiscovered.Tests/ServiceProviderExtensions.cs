using Microsoft.Extensions.DependencyInjection;

public static class ServiceProviderExtensions
{
    public static T Resolve<T>(this IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<T>();
    }
}
