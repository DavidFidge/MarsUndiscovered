using System;
using System.Collections.Generic;
using System.Linq;

using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Extensions;
using MarsUndiscovered.Interfaces;

using GoRogue.Components;
using GoRogue.GameFramework;
using GoRogue.MapGeneration;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components
{
    public class GameWorld : BaseComponent, IGameWorld
    {
        public const int MapWidth = 76;
        public const int MapHeight = 29;

        public Map Map { get; private set; }

        public Player Player { get; set; }
        public IFactory<Wall> WallFactory { get; set; }
        public IFactory<Floor> FloorFactory { get; set; }

        public List<ComponentTagPair> AllComponents { get; set; }
        public Generator Generator { get; set; }

        public void Generate()
        {
            Logger.Debug("Generating game world");

            var generator = new Generator(76, 29);

            Generator = generator.ConfigAndGenerateSafe(g => g.AddSteps(DefaultAlgorithms.DungeonMazeMapSteps()));

            AllComponents = Generator.Context.ToList();

            var wallsFloors = Generator.Context
                .GetFirst<ArrayView<bool>>()
                .ToArrayView(s => s ? (Terrain)FloorFactory.Create() : WallFactory.Create());

            Map = new Map(wallsFloors.Width, wallsFloors.Height, 1, Distance.Chebyshev);

            for (var index = 0; index < wallsFloors.Count; index++)
            {
                var gameObject = wallsFloors[index];

                gameObject.Position = Point.FromIndex(index, wallsFloors.Width);

                Map.SetTerrain(gameObject);
            }

            var floorPosition = Map.RandomPosition((p, gameObjects) => gameObjects.Any(g => g is Floor));

            Player.Position = floorPosition;

            Map.AddEntity(Player);
        }

        public Tuple<Point, Point> Move(Direction direction)
        {
            var playerPosition = Player.Position;
            var newPlayerPosition = Player.Position + direction;

            var terrain = Map.GetTerrainAt(newPlayerPosition);

            if (terrain is Floor)
                Player.Position = newPlayerPosition;

            return new Tuple<Point, Point>(playerPosition, newPlayerPosition);
        }
    }
}