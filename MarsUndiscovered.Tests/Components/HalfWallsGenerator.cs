using System.Linq;
using GoRogue.GameFramework;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components
{
    public class HalfWallsGenerator : IMapGenerator
    {
        public IMapGenerator OriginalMapGenerator { get; private set; }

        private readonly IGameObjectFactory _gameObjectFactory;
		
		public MarsMap MarsMap { get; set; }
        public int Steps { get; set; }
        public bool IsComplete { get; set; }

        public HalfWallsGenerator(IGameObjectFactory gameObjectFactory, IMapGenerator originalMapGenerator)
        {
            _gameObjectFactory = gameObjectFactory;
            OriginalMapGenerator = originalMapGenerator;
        }

        public void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            GenerateHalfWallsMap(gameWorld);
        }

        public void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            GenerateHalfWallsMap(gameWorld);
        }

        private void GenerateHalfWallsMap(IGameWorld gameWorld)
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
            IsComplete = true;
        }
    }
}