using GoRogue.GameFramework;
using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ConnectionPointSelectors;
using GoRogue.MapGeneration.Steps;
using GoRogue.MapGeneration.TunnelCreators;
using GoRogue.Random;

using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.GenerationSteps;
using MarsUndiscovered.Extensions;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Components.Maps
{
    public class MapGenerator : BaseMapGenerator
    {
        public MapGenerator()
        {
            MapWidthMin = 40;
            MapWidthMax = 50;
            MapHeightMin = 150;
            MapHeightMax = 200;
        }
        
        public override void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            Clear();
            
            var generator = new Generator(GetWidth(), GetHeight());

            var fillProbability = GlobalRandom.DefaultRNG.NextUInt(40, 60);
            var cutoffBigAreaFill = GlobalRandom.DefaultRNG.NextUInt(2, 6);
            
            var generationSteps = OutdoorGeneneration(fillProbability: (ushort)fillProbability,
                cutoffBigAreaFill: (int)cutoffBigAreaFill, border: Constants.OutdoorAreaBorder);

            ExecuteMapSteps(gameWorld, gameObjectFactory, upToStep, generator, generationSteps);
        }
        
        public override void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            Clear();

            var generator = new Generator(GetWidth(), GetHeight());

            var generationSteps = MineGeneration();

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
                .GetFirst<ArrayView<bool>>()
                .ToArrayView((s, index) =>
                    {
                        IGameObject gameObject;

                        if (s)
                        {
                            var floor = gameObjectFactory.CreateFloor();
                            floor.Index = index;
                            gameObject = floor;
                        }
                        else
                        {
                            var wall = gameObjectFactory.CreateWall();
                            wall.Index = index;
                            gameObject = wall;
                        }

                        return gameObject;
                    }
                );

            var walls = wallsFloors.ToArray().OfType<Wall>().ToList();
            var floors = wallsFloors.ToArray().OfType<Floor>().ToList();

            MarsMap = CreateMap(gameWorld, walls, floors, generator.Context.Width, generator.Context.Height);
        }

        private void Clear()
        {
            MarsMap = null;
            IsComplete = false;
            Steps = 0;
        }

        public static MarsMap CreateMap(IGameWorld gameWorld, IList<Wall> walls, IList<Floor> floors, int mapWidth, int mapHeight)
        {
            var map = new MarsMap(gameWorld, mapWidth, mapHeight);

            map.ApplyTerrainOverlay(walls, floors);

            return map;
        }

        private static IEnumerable<GenerationStep> MineGeneration(IEnhancedRandom rng = null)
        {
            rng ??= GlobalRandom.DefaultRNG;

            yield return new MineWorkerGeneration { RNG = rng };
        }

        private static IEnumerable<GenerationStep> OutdoorGeneneration(
            IEnhancedRandom rng = null,
            ushort fillProbability = 60,
            int totalIterations = 7,
            int cutoffBigAreaFill = 2,
            int border = 2,
            Distance distanceCalculation = null,
            IConnectionPointSelector connectionPointSelector = null,
            ITunnelCreator tunnelCreationMethod = null)
        {
            rng ??= GlobalRandom.DefaultRNG;
            Distance dist = distanceCalculation ?? Distance.Manhattan;
            connectionPointSelector ??= new RandomConnectionPointSelector(rng);
            tunnelCreationMethod ??= new DirectLineTunnelCreator(dist);

            // 1. Randomly fill the map with walls/floors
            yield return new RandomViewFill
            {
                FillProbability = fillProbability,
                RNG = rng,
                ExcludePerimeterPoints = false
            };

            // 2. Smooth the map into areas with the cellular automata algorithm
            yield return new CellularAutomataAreaGeneration
            {
                AreaAdjacencyRule = dist,
                TotalIterations = totalIterations,
                CutoffBigAreaFill = cutoffBigAreaFill,
            };

            // 3. Set borders to floors
            yield return new BorderGenerationStep
            {
                Border = border
            };

            // 4. Find all unique areas
            yield return new AreaFinder
            {
                AdjacencyMethod = dist
            };

            // 5. Connect areas by connecting each area to its closest neighbor
            yield return new ClosestMapAreaConnection
            {
                ConnectionPointSelector = connectionPointSelector,
                DistanceCalc = dist,
                TunnelCreator = tunnelCreationMethod
            };
        }
    }
}
