using GoRogue.GameFramework;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components
{
    public class BlankMapGenerator : IMapGenerator
    {
        private readonly IGameObjectFactory _gameObjectFactory;
        private readonly IMapGenerator _originalMapGenerator;

        public BlankMapGenerator(IGameObjectFactory gameObjectFactory, IMapGenerator originalMapGenerator)
        {
            _gameObjectFactory = gameObjectFactory;
            _originalMapGenerator = originalMapGenerator;
        }

        public ArrayView<IGameObject> CreateOutdoorWallsFloors()
        {
            var arrayView = new ArrayView<IGameObject>(MapGenerator.MapWidth, MapGenerator.MapHeight);

            for (var x = 0; x < MapGenerator.MapWidth; x++)
            {
                for (var y = 0; y < MapGenerator.MapHeight; y++)
                {
                    arrayView[x, y] = _gameObjectFactory.CreateFloor();
                }
            }

            return arrayView;
        }

        public Map CreateMap(WallCollection walls, FloorCollection floors)
        {
            return _originalMapGenerator.CreateMap(walls, floors);
        }
    }
}