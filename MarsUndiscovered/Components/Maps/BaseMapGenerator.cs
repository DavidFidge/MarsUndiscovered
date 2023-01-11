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
        public int MapWidthMin { get; set; } = 70; 
        public int MapWidthMax { get; set; } = 90; 
        
        public int MapHeightMin { get; set; } = 15; 
        public int MapHeightMax { get; set; } = 35;

        public abstract void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory,
            int? upToStep = null);

        public abstract void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory,
            int? upToStep = null);

        protected int GetWidth()
        {
            return GlobalRandom.DefaultRNG.NextInt(MapWidthMin, MapWidthMax);
        }
        
        protected int GetHeight()
        {
            return GlobalRandom.DefaultRNG.NextInt(MapHeightMin, MapHeightMax);
        }
    }
}
