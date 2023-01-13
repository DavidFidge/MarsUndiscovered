using GoRogue.GameFramework;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components
{
    public class BlankMapGenerator : BaseTestMapGenerator
    {
        public BlankMapGenerator(IGameObjectFactory gameObjectFactory, IMapGenerator originalMapGenerator) : base(gameObjectFactory, originalMapGenerator)
        {
        }
        
        public override void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            GenerateBlankMap(gameWorld);
        }

        public override void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            GenerateBlankMap(gameWorld);
        }

        private void GenerateBlankMap(IGameWorld gameWorld)
        {
            var mapWidth = GetWidth();
            var mapHeight = GetHeight();

            var arrayView = new ArrayView<IGameObject>(mapWidth, mapHeight);

            arrayView.ApplyOverlay(_ => _gameObjectFactory.CreateFloor());

            var wallsFloors = arrayView.ToArray();

            MarsMap = MapGenerator.CreateMap(gameWorld, wallsFloors.OfType<Wall>().ToList(),
                wallsFloors.OfType<Floor>().ToList(), mapWidth, mapHeight);

            Steps = 1;
            IsComplete = true;
        }
    }
}