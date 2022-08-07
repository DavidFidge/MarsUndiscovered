using System.Collections.Generic;
using System.Linq;
using FrigidRogue.MonoGame.Core.Components;

using GoRogue.GameFramework;
using GoRogue.MapGeneration;
using GoRogue.Random;

using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.GenerationSteps;
using MarsUndiscovered.Extensions;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components.Maps
{
    public class MapGenerator : BaseComponent, IMapGenerator
    {
        public MarsMap MarsMap { get; set; }
        public int Steps { get; set; }
        public bool IsComplete { get; set; }

        public void CreateOutdoorWallsFloorsMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep = null)
        {
            Clear();

            var generator = new Generator(MarsMap.MapWidth, MarsMap.MapHeight);

            var fillProbability = GlobalRandom.DefaultRNG.NextUInt(40, 60);
            var cutoffBigAreaFill = GlobalRandom.DefaultRNG.NextUInt(2, 6);
            
            var generationSteps = GeneratorAlgorithms.OutdoorGeneneration(fillProbability: (ushort)fillProbability,
                cutoffBigAreaFill: (int)cutoffBigAreaFill, border: Constants.OutdoorAreaBorder);

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

            MarsMap = CreateMap(gameWorld, walls, floors);
        }

        private void Clear()
        {
            MarsMap = null;
            IsComplete = false;
            Steps = 0;
        }

        public static MarsMap CreateMap(IGameWorld gameWorld, IList<Wall> walls, IList<Floor> floors)
        {
            var map = new MarsMap(gameWorld);

            map.ApplyTerrainOverlay(walls, floors);

            return map;
        }
    }
}
