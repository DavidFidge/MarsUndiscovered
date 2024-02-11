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

        public override void CreatePrefabMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height,
            int? upToStep = null)
        {
            GenerateHalfWallsToBlankMap(gameWorld, width, height, upToStep);
        }

        private void GenerateHalfWallsToBlankMap(IGameWorld gameWorld, int width, int height, int? upToStep)
        {
            if (upToStep != null && upToStep == 1)
            {
                var arrayView = new ArrayView<IGameObject>(width, height);

                for (var index = 0; index < arrayView.Count; index++)
                {
                    var y = index / width;
                    Terrain terrain;

                    if (y > height - 5)
                    {
                        var wall = _gameObjectFactory.CreateGameObject<Wall>();
                        wall.WallType = WallType.RockWall;
                        terrain = wall;
                    }
                    else
                    {
                        var floor = _gameObjectFactory.CreateGameObject<Floor>();
                        floor.FloorType = FloorType.RockFloor;
                        terrain = floor;
                    }
                    
                    terrain.Index = index;
                    arrayView[index] = terrain;
                }

                var wallsFloors = arrayView.ToArray();

                Map = MapGenerator.CreateMap(gameWorld, width, height)
                    .WithTerrain(wallsFloors.OfType<Wall>().ToList(), wallsFloors.OfType<Floor>().ToList());

                Steps = 1;
                IsComplete = false;
            }
            else
            {
                var arrayView = new ArrayView<IGameObject>(width, height);

                for (var index = 0; index < arrayView.Count; index++)
                {
                    var floor = _gameObjectFactory.CreateGameObject<Floor>();
                    floor.FloorType = FloorType.RockFloor;
                    floor.Index = index;
                    arrayView[index] = floor;
                }

                var wallsFloors = arrayView.ToArray();

                Map = MapGenerator.CreateMap(gameWorld, width, height)
                    .WithTerrain(wallsFloors.OfType<Wall>().ToList(), wallsFloors.OfType<Floor>().ToList());

                Steps = 2;
                IsComplete = true;
            }
        }
    }
}