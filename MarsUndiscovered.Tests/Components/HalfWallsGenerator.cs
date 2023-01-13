using GoRogue.GameFramework;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Interfaces;

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
            GenerateHalfWallsMap(gameWorld);
        }

        public override void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            GenerateHalfWallsMap(gameWorld);
        }

        private void GenerateHalfWallsMap(IGameWorld gameWorld)
        {
            var mapWidth = GetWidth();
            var mapHeight = GetHeight();

            var arrayView = new ArrayView<IGameObject>(mapWidth, mapHeight);

            var index = 0;

            arrayView.ApplyOverlay(p =>
            {
                var terrain = p.Y > mapHeight - 5
                    ? (Terrain)_gameObjectFactory.CreateWall()
                    : _gameObjectFactory.CreateFloor();
                terrain.Index = index++;
                return terrain;
            });

            var wallsFloors = arrayView.ToArray();

            MarsMap = MapGenerator.CreateMap(gameWorld, wallsFloors.OfType<Wall>().ToList(),
                wallsFloors.OfType<Floor>().ToList(), mapWidth, mapHeight);
            
            Steps = 1;
            IsComplete = true;
        }
    }
}