using GoRogue.GameFramework;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;
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
            GenerateBlankMap(gameWorld, OutdoorMapDimensions);
        }

        public override void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            GenerateBlankMap(gameWorld, BasicMapDimensions);
        }

        private void GenerateBlankMap(IGameWorld gameWorld, Point mapDimensions)
        {
            var arrayView = new ArrayView<IGameObject>(mapDimensions.X, mapDimensions.Y);

            arrayView.ApplyOverlay(_ => _gameObjectFactory.CreateFloor());

            var wallsFloors = arrayView.ToArray();

            MarsMap = MapGenerator.CreateMap(gameWorld, wallsFloors.OfType<Wall>().ToList(),
                wallsFloors.OfType<Floor>().ToList(), mapDimensions.X, mapDimensions.Y);

            Steps = 1;
            IsComplete = true;
        }
    }
}