using GoRogue.GameFramework;
using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ConnectionPointSelectors;
using GoRogue.MapGeneration.Steps;
using GoRogue.MapGeneration.TunnelCreators;
using GoRogue.Random;

using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.GenerationSteps;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class MapGenerator : BaseMapGenerator
    {
        public override void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            Clear();

            var width = 70;
            var height = 6000 / width;

            var generator = new Generator(
                width,
                GlobalRandom.DefaultRNG.NextInt(height - 5, height + 5)
                );

            var fillProbability = GlobalRandom.DefaultRNG.NextUInt(40, 60);
            var cutoffBigAreaFill = 3;

            var generationSteps = OutdoorGeneneration(
                null,
                (ushort)fillProbability,
                5,
                cutoffBigAreaFill
                );

            ExecuteMapSteps(gameWorld, gameObjectFactory, upToStep, generator, generationSteps);
        }

        public override void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            Clear();

            var width = GlobalRandom.DefaultRNG.NextInt(35, 55);

            var height = 2000 / width;

            var generator = new Generator(
                width,
                GlobalRandom.DefaultRNG.NextInt(height - 5, height + 5)
                );

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
                            var floor = gameObjectFactory.CreateGameObject<Floor>();
                            floor.Index = index;
                            gameObject = floor;
                        }
                        else
                        {
                            var wall = gameObjectFactory.CreateGameObject<Wall>();
                            wall.Index = index;
                            gameObject = wall;
                        }

                        return gameObject;
                    }
                );

            var walls = wallsFloors.ToArray().OfType<Wall>().ToList();
            var floors = wallsFloors.ToArray().OfType<Floor>().ToList();

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
            yield return new CellularAutomataOutdoorGenerator
            {
                AreaAdjacencyRule = dist,
                TotalIterations = totalIterations,
                CutoffBigAreaFill = cutoffBigAreaFill,
            };

            // 3. Find all unique areas
            yield return new AreaFinder
            {
                AdjacencyMethod = dist
            };

            // 4. Connect areas by connecting each area to its closest neighbor
            yield return new ClosestMapAreaConnection
            {
                ConnectionPointSelector = connectionPointSelector,
                DistanceCalc = dist,
                TunnelCreator = tunnelCreationMethod
            };
        }
    }
}
