using FrigidRogue.WaveFunctionCollapse;
using FrigidRogue.WaveFunctionCollapse.ContentLoaders;
using FrigidRogue.WaveFunctionCollapse.Renderers;
using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using GoRogue.MapGeneration.Steps;
using GoRogue.MapGeneration.TunnelCreators;
using GoRogue.Random;

using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.GenerationSteps;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Collections;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class MapGenerator : BaseMapGenerator
    {
        public static string WallFloorTag = "WallFloor"; // ArrayView of bool where true is floor and false is wall
        public static string TunnelsTag = "Tunnels";
        public static string WallFloorTypeTag = "WallFloorType"; // Similar to WallFloor but gives the actual type of wall or floor. It is an ArrayView of GameObjectType where the type is WallType or FloorType.
        public static string MiningFacilityAreaTag = "MiningFacilityArea";
        public static string MiningFacilityAreaWithPerimeterTag = "MiningFacilityAreaWithPerimeterTag";
        public static string DoorsTag = "Doors";
        public static string AreasTag = "Areas";
        public static string AreasWallsDoorsTag = "AreasWallsDoors";
        public static string PrefabTag = "Prefabs"; // ItemList<Prefab>

        
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

        public override void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            Clear();

            var generator = new Generator(
                width,
                GlobalRandom.DefaultRNG.NextInt(width, height)
                );

            var generationSteps = new GenerationStep[] { new MineWorkerGeneration(), new WallFloorTypeConverterGenerator() };

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

            var miningFacilityAreaFinder = new GenericAreaFinder<GameObjectType>((t) => t == FloorType.MiningFacilityFloor, MiningFacilityAreaTag, WallFloorTypeTag, areasComponentTag: AreasTag);
            
            var miningFacilityAreaWithPerimeter = new GenericAreaFinder<GameObjectType>((t) => t == FloorType.MiningFacilityFloor || t == WallType.MiningFacilityWall, MiningFacilityAreaWithPerimeterTag, WallFloorTypeTag, areasComponentTag: AreasTag);

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

            var borderGenerationStepFloors = new BorderGenerationStep("BorderFloors", true);
            borderGenerationStepFloors.Border = 3;
            
            var borderGenerationStepWalls = new BorderGenerationStep("BorderWalls", false);
            borderGenerationStepWalls.Border = 3;
            
            var generationSteps = new GenerationStep[]
            {
                // This stops prefabs being placed within 3 squares of the edge of the map, so that when tunnels are created there is more room to connect them
                // and the tunnels can fit between the edge of the map and the edge of prefabs.
                borderGenerationStepFloors,
                prefabGeneration,
                // Revert the border
                borderGenerationStepWalls,
                prefabConnectorGeneration,
                wallFloorTypeConverterGenerator
            };

            ExecuteMapSteps(gameWorld, gameObjectFactory, upToStep, generator, generationSteps);
        }

        // The generators create structures with tags. This method then extracts those
        // structures and creates a map, converting the structures to walls, floors and doors.
        private void ExecuteMapSteps(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep,
            Generator generator, IEnumerable<GenerationStep> generationSteps)
        {
            if (upToStep == null)
            {
                generator.ConfigAndGenerateSafe(g => g.AddSteps(generationSteps));
                IsComplete = true;
            }
            else
            {
                var mapStepEnumerator = generator.ConfigAndGetStageEnumeratorSafe(g => g.AddSteps(generationSteps));

                for (var i = 0; i < upToStep.Value; i++)
                {
                    Steps++;
                    var isMoreSteps = mapStepEnumerator.MoveNext();
                    if (!isMoreSteps)
                    {
                        IsComplete = true;
                        break;
                    }
                }
            }

            var wallsFloors = generator.Context
                .GetFirstOrDefault<ArrayView<GameObjectType>>(WallFloorTypeTag);

            if (wallsFloors == null)
            {
                if (upToStep == null)
                    throw new Exception("Generators did not produce an ArrayView of GameObjectType which is not allowed when not in Progressive Mode (upToStep == null)");

                var wallsFloorsBool = generator.Context
                    .GetFirst<ArrayView<bool>>(WallFloorTag)
                    .ToArray()
                    .Select(b => b ? FloorType.RockFloor : (GameObjectType)WallType.RockWall)
                    .ToArray();

                wallsFloors = new ArrayView<GameObjectType>(wallsFloorsBool, generator.Context.Width);
            }

             var walls = new List<Wall>(wallsFloors.Count);
             var floors = new List<Floor>(wallsFloors.Count);

             for (var index = 0; index < wallsFloors.Count; index++)
             {
                 var wallFloor = wallsFloors[index];

                 Terrain terrain;

                 switch (wallFloor)
                 {
                     case WallType wallType:
                     {
                         var wall = gameObjectFactory.CreateGameObject<Wall>();
                         terrain = wall;
                         wall.WallType = wallType;
                         walls.Add(wall);
                         break;
                     }
                     case FloorType floorType:
                     {
                         var floor = gameObjectFactory.CreateGameObject<Floor>();
                         terrain = floor;
                         floor.FloorType = floorType;
                         floors.Add(floor);
                         break;
                     }
                     default:
                         throw new Exception("Unknown wall/floor type");
                 }

                 terrain.Index = index;
             }
             
             var doorTypes = generator.Context
                 .GetFirstOrNew(() => new ItemList<GameObjectTypePosition<DoorType>>(), DoorsTag);

             var doors = new List<Door>(doorTypes.Count());
             
             foreach (var doorType in doorTypes.Items)
             {
                 var door = gameObjectFactory.CreateGameObject<Door>();
                 door.DoorType = doorType.GameObjectType;
                 door.Position = doorType.Position;
                 
                 doors.Add(door);
             }

             Map = CreateMap(gameWorld, generator.Context.Width, generator.Context.Height)
                .WithTerrain(walls, floors)
                .WithDoors(doors);
        }

        private void Clear()
        {
            Map = null;
            IsComplete = false;
            Steps = 0;
        }

        public static MarsMap CreateMap(IGameWorld gameWorld, int mapWidth, int mapHeight)
        {
            var map = new MarsMap(gameWorld, mapWidth, mapHeight);
            
            return map;
        }
    }
}
