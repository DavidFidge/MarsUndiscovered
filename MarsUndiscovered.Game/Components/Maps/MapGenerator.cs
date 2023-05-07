using FrigidRogue.MonoGame.Core.Interfaces.Components;
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
        public IGameProvider GameProvider { get; set; }

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

            var outdoorGeneration = new OutdoorGeneration(
                null,
                (ushort)fillProbability,
                5,
                cutoffBigAreaFill
                );

            var generationSteps = new GenerationStep[] { outdoorGeneration };

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

            var generationSteps = new GenerationStep[] { new MineWorkerGeneration() };

            ExecuteMapSteps(gameWorld, gameObjectFactory, upToStep, generator, generationSteps);
        }

        public override void CreateMiningFacilityMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            Clear();

            var width = 30;
            var height = 30;

            var generator = new Generator(width, height);

            var generationSteps = new GenerationStep[] { new MiningFacilityGeneration() };

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
    }
}
