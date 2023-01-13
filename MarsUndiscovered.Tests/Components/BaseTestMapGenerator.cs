using GoRogue.GameFramework;
using GoRogue.Random;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components
{
    public abstract class BaseTestMapGenerator : BaseMapGenerator
    {
        protected readonly IGameObjectFactory _gameObjectFactory;

        public int MapWidthMin { get; set; } = 80; 
        public int MapWidthMax { get; set; } = 81; 

        public int MapHeightMin { get; set; } = 20; 
        public int MapHeightMax { get; set; } = 21;

        public IMapGenerator OriginalMapGenerator { get; private set; }

        protected BaseTestMapGenerator(IGameObjectFactory gameObjectFactory, IMapGenerator originalMapGenerator)
        {
            MapWidthMin = 80;
            MapWidthMax = 81;
            MapHeightMin = 20;
            MapHeightMax = 21;

            _gameObjectFactory = gameObjectFactory;
            OriginalMapGenerator = originalMapGenerator;
        }

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