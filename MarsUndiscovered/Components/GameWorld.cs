using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.GenerationSteps;
using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Messages;
using MediatR;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

using Troschuetz.Random;
using Troschuetz.Random.Generators;

using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.Components
{
    public class GameWorld : BaseComponent, IGameWorld, ISaveable
    {
        public const int MapWidth = 76;
        public const int MapHeight = 29;

        public Map Map { get; private set; }
        public Player Player { get; set; }
        public IGameObjectFactory GameObjectFactory { get; set; }
        public IFactory<MoveCommand> MoveCommandFactory { get; set; }
        public IGameTurnService GameTurnService { get; set; }
        public ISaveGameStore SaveGameStore { get; set; }
        public GameObjectCollection GameObjects { get;} = new GameObjectCollection();
        public Generator Generator { get; set; }
        public MessageLog MessageLog { get; } = new MessageLog();
        public IMapper Mapper { get; set; }
        public uint Seed { get; set; }
        public string LoadGameDetail
        {
            get => $"Seed: {Seed}";
            set
            {
            }
        }

        public GameWorld()
        {
            GlobalRandom.DefaultRNG = new XorShift128Generator();
        }

        public void NewGame(uint? seed = null)
        {
            GameObjectFactory.Reset();

            if (seed == null)
                seed = TMath.Seed();

            Seed = seed.Value;

            GlobalRandom.DefaultRNG = new XorShift128Generator(seed.Value);

            Logger.Debug("Generating game world");

            Player = GameObjectFactory.CreatePlayer();
            GameObjects.Add(Player.ID, Player);

            var generator = new Generator(MapWidth, MapHeight);

            Generator = generator.ConfigAndGenerateSafe(g => g.AddSteps(GeneratorAlgorithms.OutdoorGeneneration()));

            var wallsFloors = Generator.Context
                .GetFirst<ArrayView<bool>>()
                .ToArrayView(s => s ? (Terrain)GameObjectFactory.CreateFloor() : GameObjectFactory.CreateWall());

            foreach (var item in wallsFloors.ToArray())
                GameObjects.Add(item.ID, item);

            CreateMap();

            var floorPosition = Map.RandomPosition((p, gameObjects) => gameObjects.Any(g => g is Floor));
            Player.Position = floorPosition;

            Map.AddEntity(Player);
        }

        private void CreateMap()
        {
            Debug.Assert(GameObjects.Any(), "GameObjects must be populated");

            Map = new Map(MapWidth, MapHeight, 1, Distance.Chebyshev);

            PopulateMapTerrain();
        }

        private void PopulateMapTerrain()
        {
            var wallsFloors = GameObjects
                .Values
                .OfType<Terrain>()
                .ToArrayView(MapWidth);

            Map.ApplyTerrainOverlay(wallsFloors);
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

            SaveGameStore.Clear();
            SaveGame(SaveGameStore);

            return SaveGameStore.SaveStoreToFile(saveGameName, overwrite);
        }

        public LoadGameResult LoadGame(string saveGameName)
        {
            var loadGameResult = SaveGameStore.LoadStoreFromFile(saveGameName);

            if (loadGameResult.Equals(LoadGameResult.Success))
                LoadGame(SaveGameStore);

            return loadGameResult;
        }

        public void SaveGame(ISaveGameStore saveGameStore)
        {
            GameObjectFactory.SaveGame(saveGameStore);
            GameObjects.SaveGame(saveGameStore);
            MessageLog.SaveGame(saveGameStore);

            saveGameStore.SaveToStore<GameWorld, GameWorldSaveData>(this);
        }

        public void LoadGame(ISaveGameStore saveGameStore)
        {
            saveGameStore.GetFromStore<GameWorld, GameWorldSaveData>(this);

            GameObjectFactory.LoadGame(saveGameStore);
            GameObjects.LoadGame(saveGameStore);
            MessageLog.LoadGame(saveGameStore);

            Player = (Player)GameObjects.Values.First(go => go is Player);

            CreateMap();

            PopulateMapTerrain();

            Map.AddEntity(Player);
        }
    }
}