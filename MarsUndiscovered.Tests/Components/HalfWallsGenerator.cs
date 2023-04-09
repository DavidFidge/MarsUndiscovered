using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components
{
    public class HalfWallsGenerator : BaseTestMapGenerator
    {
        public HalfWallsGenerator(IGameObjectFactory gameObjectFactory, IMapGenerator originalMapGenerator) : base(gameObjectFactory, originalMapGenerator)
        {
        }

        public override void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            GenerateHalfWallsMap(gameWorld, OutdoorMapDimensions);
        }

        public override void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            GenerateHalfWallsMap(gameWorld, BasicMapDimensions);
        }

        private void GenerateHalfWallsMap(IGameWorld gameWorld, Point mapDimensions)
        {
            var arrayView = new ArrayView<IGameObject>(mapDimensions.X, mapDimensions.Y);

            var index = 0;

            arrayView.ApplyOverlay(p =>
            {
                var terrain = p.Y > mapDimensions.Y - 5
                    ? (Terrain)_gameObjectFactory.CreateWall()
                    : _gameObjectFactory.CreateFloor();
                terrain.Index = index++;
                return terrain;
            });

            var wallsFloors = arrayView.ToArray();

            MarsMap = MapGenerator.CreateMap(gameWorld, wallsFloors.OfType<Wall>().ToList(),
                wallsFloors.OfType<Floor>().ToList(), mapDimensions.X, mapDimensions.Y);
            
            Steps = 1;
            IsComplete = true;
        }
    }
}