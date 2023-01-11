using GoRogue.GameFramework;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components
{
    public class HalfWallsToBlankMapGenerator : BaseMapGenerator
    {
        public IMapGenerator OriginalMapGenerator { get; private set; }

        private readonly IGameObjectFactory _gameObjectFactory;
        
        public HalfWallsToBlankMapGenerator(IGameObjectFactory gameObjectFactory, IMapGenerator originalMapGenerator)
        {
            MapWidthMin = 80;
            MapWidthMax = 81;
            MapHeightMin = 20;
            MapHeightMax = 21;
            
            _gameObjectFactory = gameObjectFactory;
            OriginalMapGenerator = originalMapGenerator;
        }

        public override void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            GenerateHalfWallsToBlankMap(gameWorld, upToStep);
        }

        public override void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            GenerateHalfWallsToBlankMap(gameWorld, upToStep);
        }

        private void GenerateHalfWallsToBlankMap(IGameWorld gameWorld, int? upToStep)
        {
            var mapWidth = GetWidth();
            var mapHeight = GetHeight();
            
            if (upToStep != null && upToStep == 1)
            {
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
                IsComplete = false;
            }
            else
            {
                var arrayView = new ArrayView<IGameObject>(mapWidth, mapHeight);

                arrayView.ApplyOverlay(_ => _gameObjectFactory.CreateFloor());

                var wallsFloors = arrayView.ToArray();

                MarsMap = MapGenerator.CreateMap(gameWorld, wallsFloors.OfType<Wall>().ToList(),
                    wallsFloors.OfType<Floor>().ToList(), mapWidth, mapHeight);

                Steps = 2;
                IsComplete = true;
            }
        }
    }
}