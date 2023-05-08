using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components
{
    public class BlankMapGenerator : BaseTestMapGenerator
    {
        public BlankMapGenerator(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
        }
        
        public override void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            GenerateBlankMap(gameWorld, width, height);
        }

        public override void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            GenerateBlankMap(gameWorld, width, height);
        }

        public override void CreateMiningFacilityMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height,
            int? upToStep = null)
        {
            GenerateBlankMap(gameWorld, width, height);
        }

        private void GenerateBlankMap(IGameWorld gameWorld, int width, int height)
        {
            var arrayView = new ArrayView<IGameObject>(width, height);

            arrayView.ApplyOverlay(_ => _gameObjectFactory.CreateGameObject<Floor>());

            var wallsFloors = arrayView.ToArray();

            Map = MapGenerator.CreateMap(gameWorld, wallsFloors.OfType<Wall>().ToList(),
                wallsFloors.OfType<Floor>().ToList(), width, height);

            Steps = 1;
            IsComplete = true;
        }
    }
}