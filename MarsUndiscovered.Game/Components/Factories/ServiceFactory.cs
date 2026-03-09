using FrigidRogue.MonoGame.Core.Components;
using Microsoft.Extensions.DependencyInjection;

namespace MarsUndiscovered.Game.Components.Factories
{
    public class ServiceFactory<T> : IFactory<T> where T : class
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Create()
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        public void Release(T command)
        {
        }
    }
}
