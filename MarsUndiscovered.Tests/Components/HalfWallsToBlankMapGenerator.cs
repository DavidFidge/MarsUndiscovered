using System.Linq;
using GoRogue.GameFramework;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components
{
    public class HalfWallsToBlankMapGenerator : IMapGenerator
    {
        public IMapGenerator OriginalMapGenerator { get; private set; }

        private readonly IGameObjectFactory _gameObjectFactory;

        public MarsMap MarsMap { get; set; }
        public int Steps { get; set; }
        public bool IsComplete { get; set; }

        public HalfWallsToBlankMapGenerator(IGameObjectFactory gameObjectFactory, IMapGenerator originalMapGenerator)
        {
            _gameObjectFactory = gameObjectFactory;
            OriginalMapGenerator = originalMapGenerator;
        }

        public void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            GenerateHalfWallsToBlankMap(gameWorld, upToStep);
        }

        public void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            GenerateHalfWallsToBlankMap(gameWorld, upToStep);
        }

        private void GenerateHalfWallsToBlankMap(IGameWorld gameWorld, int? upToStep)
        {
            if (upToStep != null && upToStep == 1)
            {
                var arrayView = new ArrayView<IGameObject>(MarsMap.MapWidth, MarsMap.MapHeight);

                var index = 0;

                arrayView.ApplyOverlay(p =>
                {
                    var terrain = p.Y > MarsMap.MapHeight - 5
                        ? (Terrain)_gameObjectFactory.CreateWall()
                        : _gameObjectFactory.CreateFloor();
                    terrain.Index = index++;
                    return terrain;
                });

                var wallsFloors = arrayView.ToArray();

                MarsMap = MapGenerator.CreateMap(gameWorld, wallsFloors.OfType<Wall>().ToList(),
                    wallsFloors.OfType<Floor>().ToList());
                Steps = 1;
                IsComplete = false;
            }
            else
            {
                var arrayView = new ArrayView<IGameObject>(MarsMap.MapWidth, MarsMap.MapHeight);

                arrayView.ApplyOverlay(_ => _gameObjectFactory.CreateFloor());

                var wallsFloors = arrayView.ToArray();

                MarsMap = MapGenerator.CreateMap(gameWorld, wallsFloors.OfType<Wall>().ToList(),
                    wallsFloors.OfType<Floor>().ToList());

                Steps = 2;
                IsComplete = true;
            }
        }
    }
}