using System;
using System.Collections.Generic;
using System.Linq;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;

using MarsUndiscovered.Extensions;
using MarsUndiscovered.Interfaces;

using GoRogue.GameFramework;
using GoRogue.MapGeneration;

using MarsUndiscovered.Commands;
using MarsUndiscovered.Messages;

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
        public IGameTurnService GameTurnService { get; set; }
        public IDictionary<uint, IGameObject> GameObjects { get; private set; }
        public Generator Generator { get; set; }
        public MessageLog MessageLog { get; private set; }

        public void Generate()
        {
            Logger.Debug("Generating game world");

            var generator = new Generator(76, 29);

            Generator = generator.ConfigAndGenerateSafe(g => g.AddSteps(DefaultAlgorithms.DungeonMazeMapSteps()));

            var wallsFloors = Generator.Context
                .GetFirst<ArrayView<bool>>()
                .ToArrayView(s => s ? (Terrain)FloorFactory.Create() : WallFactory.Create());

            Map = new Map(wallsFloors.Width, wallsFloors.Height, 1, Distance.Chebyshev);

            Map.ApplyTerrainOverlay(wallsFloors);

            var floorPosition = Map.RandomPosition((p, gameObjects) => gameObjects.Any(g => g is Floor));

            Player.Position = floorPosition;

            Map.AddEntity(Player);
            GameObjects = new Dictionary<uint, IGameObject>();
            GameObjects.Add(Player.ID, Player);

            MessageLog = new MessageLog();
        }

        public void MoveRequest(Direction direction)
        {
            var playerPosition = Player.Position;
            var newPlayerPosition = Player.Position + direction;
            var terrain = Map.GetTerrainAt(newPlayerPosition);

            if (terrain is Floor)
            {
                var command = new MoveCommand();

                command.Initialise(Player, new Tuple<Point, Point>(playerPosition, newPlayerPosition));
                command.Execute();

                Mediator.Send(new MapTileChangedRequest(playerPosition));
                Mediator.Send(new MapTileChangedRequest(newPlayerPosition));

                return;
            }

            MessageLog.AddMessage("The unrelenting red rock looms all around you");
        }

        public void Move(IGameObject gameObject, Point newPosition)
        {
            gameObject.Position = newPosition;
        }
    }
}