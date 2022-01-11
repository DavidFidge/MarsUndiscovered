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
                            floor.CreatedIndex = index;
                            gameObject = floor;
                        }
                        else
                        {
                            var wall = GameObjectFactory.CreateWall();
                            wall.CreatedIndex = index;
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

            var map = new Map(MapWidth, MapHeight, 1, Distance.Chebyshev);

            var wallsFloors = walls.Values.Cast<Terrain>()
                .Union(floors.Values)
                .OrderBy(t => t.CreatedIndex)
                .ToArrayView(map.Width);

            map.ApplyTerrainOverlay(wallsFloors);

            return map;
        }
    }
}
