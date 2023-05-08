using FrigidRogue.WaveFunctionCollapse;
using FrigidRogue.WaveFunctionCollapse.ContentLoaders;
using FrigidRogue.WaveFunctionCollapse.Renderers;
using GoRogue.GameFramework;
using GoRogue.MapGeneration;
using GoRogue.Random;

using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.GenerationSteps;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class MapGenerator : BaseMapGenerator
    {
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
                null,
                (ushort)fillProbability,
                5,
                cutoffBigAreaFill
                );

            var generationSteps = new GenerationStep[] { outdoorGeneration };

            ExecuteMapSteps(gameWorld, gameObjectFactory, upToStep, generator, generationSteps);
        }

        public override void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null)
        {
            Clear();

            var generator = new Generator(
                width,
                GlobalRandom.DefaultRNG.NextInt(width, height)
                );

            var generationSteps = new GenerationStep[] { new MineWorkerGeneration() };

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
                .GetFirst<ArrayView<bool>>("WallFloor")
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
    }
}
