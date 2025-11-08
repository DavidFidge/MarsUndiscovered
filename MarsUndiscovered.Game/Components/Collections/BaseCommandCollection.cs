using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public abstract class BaseCommandCollection<T> : List<BaseMarsGameActionCommand>
        where T : BaseMarsGameActionCommand
    {
        protected readonly IGameWorld GameWorld;

        public BaseCommandCollection(IGameWorld gameWorld)
        {
            GameWorld = gameWorld;
        }

        protected abstract T Create();
    }
}
