using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components.Maps
{
    public abstract class BaseMapGenerator : BaseComponent, IMapGenerator
    {
        public MarsMap Map { get; set; }
        public int Steps { get; set; }
        public bool IsComplete { get; set; }

        public abstract void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null);

        public abstract void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null);

        public abstract void CreateMiningFacilityMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null);
    }
}
