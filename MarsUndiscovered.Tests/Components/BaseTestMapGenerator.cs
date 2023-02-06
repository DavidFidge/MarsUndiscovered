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

        public Point BasicMapDimensions { get; set; } = new Point(20, 20);
        public Point OutdoorMapDimensions { get; set; } = new Point(70, 85);

        public IMapGenerator OriginalMapGenerator { get; private set; }

        protected BaseTestMapGenerator(IGameObjectFactory gameObjectFactory, IMapGenerator originalMapGenerator)
        {
            _gameObjectFactory = gameObjectFactory;
            OriginalMapGenerator = originalMapGenerator;
        }
    }
}