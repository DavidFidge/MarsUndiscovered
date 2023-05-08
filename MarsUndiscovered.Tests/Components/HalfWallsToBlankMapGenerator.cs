using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components
{
    public class HalfWallsToBlankMapGenerator : BaseTestMapGenerator
    {
        public HalfWallsToBlankMapGenerator(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
        }

        public override void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            GenerateHalfWallsToBlankMap(gameWorld, width, height, upToStep);
        }

        public override void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            GenerateHalfWallsToBlankMap(gameWorld, width, height, upToStep);
        }

        public override void CreateMiningFacilityMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height,
            int? upToStep = null)
        {
            GenerateHalfWallsToBlankMap(gameWorld, width, height, upToStep);
        }

        private void GenerateHalfWallsToBlankMap(IGameWorld gameWorld, int width, int height, int? upToStep)
        {
            if (upToStep != null && upToStep == 1)
            {
                var arrayView = new ArrayView<IGameObject>(width, height);

                var index = 0;

                arrayView.ApplyOverlay(p =>
                {
                    var terrain = p.Y > height - 5
                        ? (Terrain)_gameObjectFactory.CreateGameObject<Wall>()
                        : _gameObjectFactory.CreateGameObject<Floor>();
                    terrain.Index = index++;
                    return terrain;
                });

                var wallsFloors = arrayView.ToArray();

                Map = MapGenerator.CreateMap(gameWorld, wallsFloors.OfType<Wall>().ToList(),
                    wallsFloors.OfType<Floor>().ToList(), width, height);
                Steps = 1;
                IsComplete = false;
            }
            else
            {
                var arrayView = new ArrayView<IGameObject>(width, height);

                arrayView.ApplyOverlay(_ => _gameObjectFactory.CreateGameObject<Floor>());

                var wallsFloors = arrayView.ToArray();

                Map = MapGenerator.CreateMap(gameWorld, wallsFloors.OfType<Wall>().ToList(),
                    wallsFloors.OfType<Floor>().ToList(), width, height);

                Steps = 2;
                IsComplete = true;
            }
        }
    }
}