using FrigidRogue.WaveFunctionCollapse;
using FrigidRogue.WaveFunctionCollapse.ContentLoaders;
using FrigidRogue.WaveFunctionCollapse.Renderers;
using GoRogue.MapGeneration;
using GoRogue.Random;

using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.GenerationSteps;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class MapGenerator : BaseMapGenerator
    {
        public static string WallFloorTag = "WallFloor";
        public static string TunnelTag = "Tunnel";
        public static string WallFloorTypeTag = "WallFloorType";
        
        private readonly IWaveFunctionCollapseGeneratorPasses _waveFunctionCollapseGeneratorPasses;
        private readonly IWaveFunctionCollapseGeneratorPassesContentLoader _waveFunctionCollapseGeneratorPassesContentLoader;
        private readonly IWaveFunctionCollapseGeneratorPassesRenderer _waveFunctionCollapseGeneratorPassesRenderer;

        public MapGenerator(
            IWaveFunctionCollapseGeneratorPasses waveFunctionCollapseGeneratorPasses,
            IWaveFunctionCollapseGeneratorPassesContentLoader waveFunctionCollapseGeneratorPassesContentLoader,
            IWaveFunctionCollapseGeneratorPassesRenderer waveFunctionCollapseGeneratorPassesRenderer
            )
        {
            _waveFunctionCollapseGeneratorPasses = waveFunctionCollapseGeneratorPasses;
            _waveFunctionCollapseGeneratorPassesContentLoader = waveFunctionCollapseGeneratorPassesContentLoader;
            _waveFunctionCollapseGeneratorPassesRenderer = waveFunctionCollapseGeneratorPassesRenderer;
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

            var generationSteps = new GenerationStep[]
            {
                new MiningFacilityGeneration(
                    _waveFunctionCollapseGeneratorPasses,
                    _waveFunctionCollapseGeneratorPassesContentLoader,
                    _waveFunctionCollapseGeneratorPassesRenderer
                )
            };

            ExecuteMapSteps(gameWorld, gameObjectFactory, upToStep, generator, generationSteps);
        }

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
                .GetFirst<ArrayView<GameObjectType>>(WallFloorTypeTag);
                
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

            Map = CreateMap(gameWorld, walls, floors, generator.Context.Width, generator.Context.Height);
        }

        private void Clear()
        {
            Map = null;
            IsComplete = false;
            Steps = 0;
        }

        public static MarsMap CreateMap(IGameWorld gameWorld, IList<Wall> walls, IList<Floor> floors, int mapWidth, int mapHeight)
        {
            var map = new MarsMap(gameWorld, mapWidth, mapHeight);

            map.ApplyTerrainOverlay(walls, floors);

            return map;
        }
    }
}
