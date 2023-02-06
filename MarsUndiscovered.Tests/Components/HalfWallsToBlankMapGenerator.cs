using GoRogue.GameFramework;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components
{
    public class HalfWallsToBlankMapGenerator : BaseTestMapGenerator
    {
        public HalfWallsToBlankMapGenerator(IGameObjectFactory gameObjectFactory, IMapGenerator originalMapGenerator) : base(gameObjectFactory, originalMapGenerator)
        {
        }

        public override void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            GenerateHalfWallsToBlankMap(gameWorld, OutdoorMapDimensions, upToStep);
        }

        public override void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            GenerateHalfWallsToBlankMap(gameWorld, BasicMapDimensions, upToStep);
        }

        private void GenerateHalfWallsToBlankMap(IGameWorld gameWorld, Point mapDimensions, int? upToStep)
        {
            if (upToStep != null && upToStep == 1)
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
                IsComplete = false;
            }
            else
            {
                var arrayView = new ArrayView<IGameObject>(mapDimensions.X, mapDimensions.Y);

                arrayView.ApplyOverlay(_ => _gameObjectFactory.CreateFloor());

                var wallsFloors = arrayView.ToArray();

                MarsMap = MapGenerator.CreateMap(gameWorld, wallsFloors.OfType<Wall>().ToList(),
                    wallsFloors.OfType<Floor>().ToList(), mapDimensions.X, mapDimensions.Y);

                Steps = 2;
                IsComplete = true;
            }
        }
    }
}