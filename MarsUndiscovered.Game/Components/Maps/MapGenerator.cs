using FrigidRogue.WaveFunctionCollapse;
using FrigidRogue.WaveFunctionCollapse.ContentLoaders;
using FrigidRogue.WaveFunctionCollapse.Renderers;
using GoRogue.MapGeneration;
using GoRogue.Random;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.GenerationSteps;
using MarsUndiscovered.Interfaces;
using ShaiRandom.Collections;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class MapGenerator : BaseMapGenerator
    {
        private readonly IWaveFunctionCollapseGeneratorPasses _waveFunctionCollapseGeneratorPasses;
        private readonly IWaveFunctionCollapseGeneratorPassesContentLoader _waveFunctionCollapseGeneratorPassesContentLoader;
        private readonly IWaveFunctionCollapseGeneratorPassesRenderer _waveFunctionCollapseGeneratorPassesRenderer;
        private readonly IPrefabProvider _prefabProvider;

        public MapGenerator(
            IWaveFunctionCollapseGeneratorPasses waveFunctionCollapseGeneratorPasses,
            IWaveFunctionCollapseGeneratorPassesContentLoader waveFunctionCollapseGeneratorPassesContentLoader,
            IWaveFunctionCollapseGeneratorPassesRenderer waveFunctionCollapseGeneratorPassesRenderer,
            IPrefabProvider prefabProvider
            )
        {
            _waveFunctionCollapseGeneratorPasses = waveFunctionCollapseGeneratorPasses;
            _waveFunctionCollapseGeneratorPassesContentLoader = waveFunctionCollapseGeneratorPassesContentLoader;
            _waveFunctionCollapseGeneratorPassesRenderer = waveFunctionCollapseGeneratorPassesRenderer;
            _prefabProvider = prefabProvider;
        }

        public override void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            Clear();

            var generator = new Generator(
                width,
                height
                );

            var fillProbability = GlobalRandom.DefaultRNG.NextUInt(40, 60);
            var cutoffBigAreaFill = 3;

            var outdoorGeneration = new OutdoorGeneration(
                (ushort)fillProbability,
                5,
                cutoffBigAreaFill
                );

            var generationSteps = outdoorGeneration.GetSteps();

            ExecuteMapSteps(gameWorld, gameObjectFactory, upToStep, generator, generationSteps);
        }

        public override void CreateMineMapWithCanteen(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            Clear();

            var generator = new Generator(
                width,
                GlobalRandom.DefaultRNG.NextInt(width, height)
                );

            var generationSteps = new GenerationStep[]
            {
                new MineWorkerGeneration(),
                new WallFloorTypeConverterGenerator(),
                new HoleInTheWallGenerator(5, 5, 12, 12),
                new CanteenGenerator()
            };

            ExecuteMapSteps(gameWorld, gameObjectFactory, upToStep, generator, generationSteps);
        }

        public override void CreateMineMapWithHoleInTheRubble(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            Clear();

            var generator = new Generator(
                width,
                GlobalRandom.DefaultRNG.NextInt(width, height)
                );

            var generationSteps = new GenerationStep[]
            {
                new MineWorkerGeneration(),
                new WallFloorTypeConverterGenerator(),
                new HoleInTheWallGenerator(4, 4, 6, 6),
                new HoleToUnderground()
            };

            ExecuteMapSteps(gameWorld, gameObjectFactory, upToStep, generator, generationSteps);
        }

        public override void CreateMiningFacilityMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            Clear();

            var generator = new Generator(width, height);

            var miningFacilityGeneration = new MiningFacilityGeneration(
                _waveFunctionCollapseGeneratorPasses,
                _waveFunctionCollapseGeneratorPassesContentLoader,
                _waveFunctionCollapseGeneratorPassesRenderer
            );

            var miningFacilityAreaFinder = new GenericAreaFinder<GameObjectType>(t => t == FloorType.MiningFacilityFloor, MiningFacilityAreaTag, WallFloorTypeTag, areasComponentTag: AreasTag);
            
            var miningFacilityAreaWithPerimeter = new GenericAreaFinder<GameObjectType>(t => t == FloorType.MiningFacilityFloor || t == WallType.MiningFacilityWall, MiningFacilityAreaWithPerimeterTag, WallFloorTypeTag, areasComponentTag: AreasTag);

            var probabilityTable = new ProbabilityTable<int>(
                new List<(int item, double weight)> { (1, 5), (2, 1) }
                );

            var internalWallsGeneration = new InternalWallsGeneration(WallType.MiningFacilityWall, DoorType.DefaultDoor, probabilityTable, splitFactor: 12);
            internalWallsGeneration.AreasStepFilterTag = MiningFacilityAreaTag;
            
            var areaPerimeterDoorGeneration = new AreaPerimeterDoorGeneration(FloorType.MiningFacilityFloor,
                DoorType.DefaultDoor, minDoors: 1, maxDoors: 4);

            areaPerimeterDoorGeneration.AreasStepFilterTag = MiningFacilityAreaWithPerimeterTag;
            
            var generationSteps = new GenerationStep[]
            {
                miningFacilityGeneration,
                miningFacilityAreaFinder,
                miningFacilityAreaWithPerimeter,
                internalWallsGeneration,
                areaPerimeterDoorGeneration
            };

            ExecuteMapSteps(gameWorld, gameObjectFactory, upToStep, generator, generationSteps);
        }

        public override void CreatePrefabMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            Clear();

            var generator = new Generator(width, height);
            var prefabGeneration = new PrefabGeneration(_prefabProvider);
            
            var prefabConnectorGeneration = new PrefabConnectorGeneration();
            var wallFloorTypeConverterGenerator = new WallFloorTypeConverterGenerator();

            var borderGenerationStepFloors = new BorderGenerationStep("BorderFloors");
            borderGenerationStepFloors.Border = 3;
            
            var borderGenerationStepWalls = new BorderGenerationStep("BorderWalls", false);
            borderGenerationStepWalls.Border = 3;

            var doorGenerator = new PrefabDoorGenerator();
            
            var generationSteps = new GenerationStep[]
            {
                // This stops prefabs being placed within 3 squares of the edge of the map, so that when tunnels are created there is more room to connect them
                // and the tunnels can fit between the edge of the map and the edge of prefabs.
                borderGenerationStepFloors,
                prefabGeneration,
                // Revert the border
                borderGenerationStepWalls,
                prefabConnectorGeneration,
                wallFloorTypeConverterGenerator,
                doorGenerator
            };

            ExecuteMapSteps(gameWorld, gameObjectFactory, upToStep, generator, generationSteps);
        }
    }
}
