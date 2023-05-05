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

        public SpecificMapGenerator(IGameObjectFactory gameObjectFactory, IMapGenerator originalMapGenerator, IList<Point> wallPoints) : base(gameObjectFactory, originalMapGenerator)
        {
            var index = 0;

            _terrainChooser = ((p) =>
            {
                var terrain = wallPoints.Contains(p) ? (Terrain)_gameObjectFactory.CreateGameObject<Wall>() : _gameObjectFactory.CreateGameObject<Floor>();
                terrain.Index = index++;
                return terrain;
            });
        }

        public override void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            GenerateSpecificMap(gameWorld, OutdoorMapDimensions);
        }

        public override void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            GenerateSpecificMap(gameWorld, BasicMapDimensions);
        }

        private void GenerateSpecificMap(IGameWorld gameWorld, Point mapDimensions)
        {
            var arrayView = new ArrayView<IGameObject>(mapDimensions.X, mapDimensions.Y);

            arrayView.ApplyOverlay(_terrainChooser);

            var wallsFloors = arrayView.ToArray();

            Map = MapGenerator.CreateMap(gameWorld, wallsFloors.OfType<Wall>().ToList(),
                wallsFloors.OfType<Floor>().ToList(), mapDimensions.X, mapDimensions.Y);
            
            Steps = 1;
            IsComplete = true;
        }
    }
}