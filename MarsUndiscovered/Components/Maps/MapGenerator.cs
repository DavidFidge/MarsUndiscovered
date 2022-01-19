using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static ArrayView<IGameObject> CreateOutdoorWallsFloors(IGameObjectFactory gameObjectFactory)
        {
            var generator = new Generator(MarsMap.MapWidth, MarsMap.MapHeight);

            var fillProbability = GlobalRandom.DefaultRNG.NextUInt(40, 60);
            var cutoffBigAreaFill = GlobalRandom.DefaultRNG.NextUInt(2, 6);

            generator.ConfigAndGenerateSafe(g => g.AddSteps(GeneratorAlgorithms.OutdoorGeneneration(fillProbability: (ushort)fillProbability, cutoffBigAreaFill: (int)cutoffBigAreaFill)));

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

            return wallsFloors;
        }

        public MarsMap CreateMap(IGameWorld gameWorld, IList<Wall> walls, IList<Floor> floors)
        {
            var map = new MarsMap(gameWorld);

            map.ApplyTerrainOverlay(walls, floors);

            return map;
        }

        public MarsMap CreateMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, Func<IGameObjectFactory, ArrayView<IGameObject>> wallsFloorsGenerator)
        {
            var wallsFloors = wallsFloorsGenerator(gameObjectFactory);

            var walls = wallsFloors.ToArray().OfType<Wall>().ToList();
            var floors = wallsFloors.ToArray().OfType<Floor>().ToList();

            return CreateMap(gameWorld, walls, floors);
        }
    }
}
