using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components
{
    public class SpecificMapGenerator : BaseTestMapGenerator
    {
        private readonly HashSet<Point> _wallPoints;

        public SpecificMapGenerator(IGameObjectFactory gameObjectFactory, IList<Point> wallPoints) : base(gameObjectFactory)
        {
            _wallPoints = wallPoints.ToHashSet();
        }

        public override void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            GenerateSpecificMap(gameWorld, width, height);
        }

        public override void CreateMineMapWithCanteen(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            GenerateSpecificMap(gameWorld, width, height);
        }

        public override void CreateMiningFacilityMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height,
            int? upToStep = null)
        {
            GenerateSpecificMap(gameWorld, width, height);
        }

        public override void CreatePrefabMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height,
            int? upToStep = null)
        {
            GenerateSpecificMap(gameWorld, width, height);
        }

        private void GenerateSpecificMap(IGameWorld gameWorld, int width, int height)
        {
            var arrayView = new ArrayView<IGameObject>(width, height);

            for (var index = 0; index < arrayView.Count; index++)
            {
                var point = Point.FromIndex(index, width);
                Terrain terrain;

                if (_wallPoints.Contains(point))
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

            Map = CreateMap(gameWorld, width, height)
                .WithTerrain(wallsFloors.OfType<Wall>().ToList(), wallsFloors.OfType<Floor>().ToList());
            
            Steps = 1;
            IsComplete = true;
        }

        public override void CreateMineMapWithHoleInTheRubble(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            GenerateSpecificMap(gameWorld, width, height);
        }
    }
}