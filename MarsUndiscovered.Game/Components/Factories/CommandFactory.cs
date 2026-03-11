using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.DependencyInjection;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components.Factories
{
    public class CommandFactory<T> : ICommandFactory<T> where T : BaseGameActionCommand
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Create(IGameWorld gameWorld)
        {
            return _serviceProvider.CreateWithInjectedProperties<T>(gameWorld);
        }

        public void Release(T command)
        {
        }
    }
}
