using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using MarsUndiscovered.Extensions;
using MarsUndiscovered.Interfaces;

using GoRogue.GameFramework;
using GoRogue.MapGeneration;
using GoRogue.Random;

using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Messages;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

using Troschuetz.Random;
using Troschuetz.Random.Generators;

namespace MarsUndiscovered.Components
{
    public class GameWorld : BaseComponent, IGameWorld, ISaveable
    {
        public const int MapWidth = 76;
        public const int MapHeight = 29;

        public Map Map { get; private set; }
        public Player Player { get; set; }
        public IFactory<Player> PlayerFactory { get; set; }
        public IFactory<Wall> WallFactory { get; set; }
        public IFactory<Floor> FloorFactory { get; set; }
        public IFactory<MoveCommand> MoveCommandFactory { get; set; }
        public IGameTurnService GameTurnService { get; set; }
        public ISaveGameService SaveGameService { get; set; }
        public ISaveGameStore SaveGameStore { get; set; }
        public IDictionary<uint, IGameObject> GameObjects { get; private set; }
        public Generator Generator { get; set; }
        public MessageLog MessageLog { get; private set; }
        public IMapper Mapper { get; set; }

        public uint Seed { get; set; }

        public void Generate(uint? seed = null)
        {
            if (seed == null)
                seed = TMath.Seed();

            Seed = seed.Value;

            GlobalRandom.DefaultRNG = new XorShift128Generator(seed.Value);

            Logger.Debug("Generating game world");

            var generator = new Generator(MapWidth, MapHeight);

            Generator = generator.ConfigAndGenerateSafe(g => g.AddSteps(DefaultAlgorithms.DungeonMazeMapSteps()));

            var wallsFloors = Generator.Context
                .GetFirst<ArrayView<bool>>()
                .ToArrayView(s => s ? (Terrain)FloorFactory.Create() : WallFactory.Create());

            Map = CreateMap();

            Map.ApplyTerrainOverlay(wallsFloors);

            var floorPosition = Map.RandomPosition((p, gameObjects) => gameObjects.Any(g => g is Floor));

            Player = PlayerFactory.Create();
            Player.Position = floorPosition;

            Map.AddEntity(Player);
            GameObjects = new Dictionary<uint, IGameObject>();
            GameObjects.Add(Player.ID, Player);

            foreach (var item in wallsFloors.ToArray())
                GameObjects.Add(item.ID, item);

            MessageLog = new MessageLog();
        }

        private Map CreateMap()
        {
            return new Map(MapWidth, MapHeight, 1, Distance.Chebyshev);
        }

        public void MoveRequest(Direction direction)
        {
            var playerPosition = Player.Position;
            var newPlayerPosition = Player.Position + direction;

            if (Map.GameObjectCanMove(Player, newPlayerPosition))
            {
                var terrainAtDestination = Map.GetTerrainAt(newPlayerPosition);

                if (terrainAtDestination is Floor)
                {
                    var command = MoveCommandFactory.Create();

                    command.Initialise(Player, new Tuple<Point, Point>(playerPosition, newPlayerPosition));
                    command.Execute();

                    Mediator.Send(new MapTileChangedRequest(playerPosition));
                    Mediator.Send(new MapTileChangedRequest(newPlayerPosition));
                }
                else if (terrainAtDestination is Wall)
                {
                    MessageLog.AddMessage("The unrelenting red rock is cold and dry");
                }
            }
        }

        public IList<string> GetMessagesSince(int currentCount)
        {
            if (currentCount == MessageLog.Count)
                return Array.Empty<string>();

            return MessageLog
                .Skip(currentCount)
                .Select(s => s.Message)
                .ToList();
        }

        public SaveGameResult SaveGame(string saveGameName, bool overwrite)
        {
            if (!overwrite)
            {
                var canSaveGameResult = SaveGameStore.CanSaveStoreToFile(saveGameName);
                if (canSaveGameResult.Equals(SaveGameResult.Overwrite))
                    return canSaveGameResult;
            }

            var gameObjectSaveData = Mapper.Map<IList<IGameObject>, IList<GameObjectSaveData>>(GameObjects.Values.ToList());

            SaveGameStore.SaveToStore(gameObjectSaveData.CreateMemento());

            return SaveGameStore.SaveStoreToFile(saveGameName, overwrite);
        }

        public void LoadGame(string saveGameName)
        {
            SaveGameStore.LoadStoreFromFile(saveGameName);

            var gameObjectSaveData = SaveGameStore.GetFromStore<IList<GameObjectSaveData>>();

            var gameObjects = Mapper.Map<IList<GameObjectSaveData>, IList<IGameObject>>(gameObjectSaveData.State);

            GameObjects = gameObjects.ToDictionary(k => k.ID, v => v);

            Map = CreateMap();

            var wallsFloors = GameObjects
                .Values
                .OfType<Terrain>()
                .ToArrayView(MapWidth);

            Map.ApplyTerrainOverlay(wallsFloors);
        }

        public void SaveGame(ISaveGameStore saveGameStore)
        {
            saveGameStore.SaveToStore<GameWorldSaveData>(new Memento<GameWorldSaveData>(Mapper.Map()));

        }

        public void LoadGame(ISaveGameStore saveGameStore)
        {
            throw new NotImplementedException();
        }
    }
}