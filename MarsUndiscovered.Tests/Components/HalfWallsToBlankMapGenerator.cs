using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps;
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
                        ? (Terrain)_gameObjectFactory.CreateGameObject<Wall>()
                        : _gameObjectFactory.CreateGameObject<Floor>();
                    terrain.Index = index++;
                    return terrain;
                });

                var wallsFloors = arrayView.ToArray();

                Map = MapGenerator.CreateMap(gameWorld, wallsFloors.OfType<Wall>().ToList(),
                    wallsFloors.OfType<Floor>().ToList(), mapDimensions.X, mapDimensions.Y);
                Steps = 1;
                IsComplete = false;
            }
            else
            {
                var arrayView = new ArrayView<IGameObject>(mapDimensions.X, mapDimensions.Y);

                arrayView.ApplyOverlay(_ => _gameObjectFactory.CreateGameObject<Floor>());

                var wallsFloors = arrayView.ToArray();

                Map = MapGenerator.CreateMap(gameWorld, wallsFloors.OfType<Wall>().ToList(),
                    wallsFloors.OfType<Floor>().ToList(), mapDimensions.X, mapDimensions.Y);

                Steps = 2;
                IsComplete = true;
            }
        }
    }
}