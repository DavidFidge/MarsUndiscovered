using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components
{
    public abstract class BaseTestMapGenerator : BaseMapGenerator
    {
        protected readonly IGameObjectFactory _gameObjectFactory;

        public Point BasicMapDimensions { get; set; } = new Point(20, 20);
        public Point OutdoorMapDimensions { get; set; } = new Point(70, 85);

        protected BaseTestMapGenerator(IGameObjectFactory gameObjectFactory)
        {
            _gameObjectFactory = gameObjectFactory;
        }
    }
}