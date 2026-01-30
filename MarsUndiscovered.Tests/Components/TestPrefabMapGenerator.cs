using GoRogue.MapGeneration;
using GoRogue.Random;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.GenerationSteps;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Interfaces;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Tests.Components
{
    public class TestPrefabMapGenerator : BaseTestMapGenerator
    {
        private TestPrefabProvider _testPrefabProvider;
        public IEnhancedRandom RNG { get; set; } = GlobalRandom.DefaultRNG;

        public TestPrefabMapGenerator(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
            _testPrefabProvider = new TestPrefabProvider();
        }

        public override void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            GenerateTestMap(gameWorld, width, height);
        }

        public override void CreateMineMapWithCanteen(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            GenerateTestMap(gameWorld, width, height);
        }

        public override void CreateMiningFacilityMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height,
            int? upToStep = null)
        {
            GenerateTestMap(gameWorld, width, height);
        }

        public override void CreatePrefabMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height,
            int? upToStep = null)
        {
            GenerateTestMap(gameWorld, width, height);
        }

        private void GenerateTestMap(IGameWorld gameWorld, int width, int height)
        {
            var generator = new Generator(width, height);
            var prefabGeneration = new PrefabGeneration(_testPrefabProvider);
            
            var prefabConnectorGeneration = new PrefabConnectorGeneration();
            var wallFloorTypeConverterGenerator = new WallFloorTypeConverterGenerator();

            var generationSteps = new GenerationStep[]
            {
                prefabGeneration,
                prefabConnectorGeneration,
                wallFloorTypeConverterGenerator
            };

            ExecuteMapSteps(gameWorld, _gameObjectFactory, null, generator, generationSteps);
        }

        public override void CreateMineMapWithHoleInTheRubble(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            GenerateTestMap(gameWorld, width, height);
        }
    }
}