using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

using Newtonsoft.Json;

namespace MarsUndiscovered.Commands
{
    public abstract class BaseMarsGameActionCommand<T> : BaseStatefulGameActionCommand<T>
    {
        [JsonIgnore]
        public IGameWorld GameWorld { get; private set; }

        [JsonIgnore]
        public ICommandFactory CommandFactory { get; set; }

        public void SetGameWorld(IGameWorld gameWorld)
        {
            GameWorld = gameWorld;
        }
    }
}
