using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FrigidRogue.MonoGame.Core.Components;
using GoRogue.GameFramework;
using GoRogue.MapGeneration;
using GoRogue.Random;

using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.GenerationSteps;
using MarsUndiscovered.Extensions;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components.Maps
{
    public class MapGenerator : BaseComponent, IMapGenerator
    {
        public const int MapWidth = 84;
        public const int MapHeight = 26;

        public IGameObjectFactory GameObjectFactory { get; set; }

        public ArrayView<IGameObject> CreateOutdoorWallsFloors()
        {
            var generator = new Generator(MapWidth, MapHeight);

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
                            var floor = GameObjectFactory.CreateFloor();
                            floor.Index = index;
                            gameObject = floor;
                        }
                        else
                        {
                            var wall = GameObjectFactory.CreateWall();
                            wall.Index = index;
                            gameObject = wall;
                        }

                        return gameObject;
                    }
                );

            return wallsFloors;
        }

        public Map CreateMap(WallCollection walls, FloorCollection floors)
        {
            Debug.Assert(floors.Any() || walls.Any(), "Walls and/or Floors must be populated");

            var map = new Map(MapWidth, MapHeight, 2, Distance.Chebyshev, UInt32.MaxValue, 1, 0);

            var wallsFloors = walls.Values.Cast<Terrain>()
                .Union(floors.Values)
                .Where(t => !t.IsDestroyed)
                .OrderBy(t => t.Index)
                .ToArrayView(map.Width);

            map.ApplyTerrainOverlay(wallsFloors);

            return map;
        }
    }
}
