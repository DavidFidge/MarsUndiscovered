using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components
{
    public class SpecificMapGenerator : BaseTestMapGenerator
    {
        private readonly Func<Point, IGameObject> _terrainChooser;

        public SpecificMapGenerator(IGameObjectFactory gameObjectFactory, IList<Point> wallPoints) : base(gameObjectFactory)
        {
            var index = 0;

            _terrainChooser = ((p) =>
            {
                var terrain = wallPoints.Contains(p) ? (Terrain)_gameObjectFactory.CreateGameObject<Wall>() : _gameObjectFactory.CreateGameObject<Floor>();
                terrain.Index = index++;
                return terrain;
            });
        }

        public override void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            GenerateSpecificMap(gameWorld, width, height);
        }

        public override void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            GenerateSpecificMap(gameWorld, width, height);
        }

        public override void CreateMiningFacilityMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height,
            int? upToStep = null)
        {
            GenerateSpecificMap(gameWorld, width, height);
        }

        private void GenerateSpecificMap(IGameWorld gameWorld, int width, int height)
        {
            var arrayView = new ArrayView<IGameObject>(width, height);

            arrayView.ApplyOverlay(_terrainChooser);

            var wallsFloors = arrayView.ToArray();

            Map = MapGenerator.CreateMap(gameWorld, wallsFloors.OfType<Wall>().ToList(),
                wallsFloors.OfType<Floor>().ToList(), width, height);
            
            Steps = 1;
            IsComplete = true;
        }
    }
}