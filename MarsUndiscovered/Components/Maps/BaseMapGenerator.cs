using FrigidRogue.MonoGame.Core.Components;

using GoRogue.Random;

using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components.Maps
{
    public abstract class BaseMapGenerator : BaseComponent, IMapGenerator
    {
        public MarsMap MarsMap { get; set; }
        public int Steps { get; set; }
        public bool IsComplete { get; set; }

        public abstract void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory,
            int? upToStep = null);

        public abstract void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory,
            int? upToStep = null);
    }
}
