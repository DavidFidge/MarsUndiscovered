using System.Reflection;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace MarsUndiscovered.Game.DependencyInjection
{
    public static class ServiceProviderFactoryExtensions
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> SettablePropertiesCache = new();

        public static IServiceCollection AddTransientWithProperties<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddTransient<TService>(sp => (TService)sp.CreateWithInjectedProperties(typeof(TImplementation)));
            return services;
        }

        public static IServiceCollection AddSingletonWithProperties<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddSingleton<TService>(sp => (TService)sp.CreateWithInjectedProperties(typeof(TImplementation)));
            return services;
        }

        public static T CreateWithInjectedProperties<T>(this IServiceProvider serviceProvider, params object[] parameters)
            where T : class
        {
            return (T)serviceProvider.CreateWithInjectedProperties(typeof(T), parameters);
        }

        public static object CreateWithInjectedProperties(this IServiceProvider serviceProvider, Type implementationType, params object[] parameters)
        {
            var instance = ActivatorUtilities.CreateInstance(serviceProvider, implementationType, parameters);
            InjectSettableProperties(serviceProvider, instance);
            return instance;
        }

        public static void InjectSettableProperties(this IServiceProvider serviceProvider, object instance)
        {
            var properties = SettablePropertiesCache.GetOrAdd(
                instance.GetType(),
                type => type
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0)
                    .ToArray());

            foreach (var property in properties)
            {
                if (property.GetValue(instance) != null)
                    continue;

                var value = serviceProvider.GetService(property.PropertyType);

                if (value != null)
                    property.SetValue(instance, value);
            }
        }
    }
}
