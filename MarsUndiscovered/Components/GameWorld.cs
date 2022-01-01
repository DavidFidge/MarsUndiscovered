using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
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

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using Serilog.Data;
using Troschuetz.Random;
using Troschuetz.Random.Generators;

namespace MarsUndiscovered.Components
{
    public class GameWorld : BaseComponent, IGameWorld, ISaveable, IMementoState<GameWorldSaveData>
    {
        public const int MapWidth = 76;
        public const int MapHeight = 29;

        public Map Map { get; private set; }
        public Player Player { get; set; }
        public IGameObjectFactory GameObjectFactory { get; set; }
        public ICommandFactory CommandFactory { get; set; }
        public IGameTurnService GameTurnService { get; set; }
        public ISaveGameStore SaveGameStore { get; set; }
        public WallCollection Walls { get; private set; }
        public FloorCollection Floors { get; private set; }
        public MonsterCollection Monsters { get; private set; }
        public CommandCollection HistoricalCommands { get; private set; }
        public IDictionary<uint, IGameObject> GameObjects => GameObjectFactory.GameObjects;
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
            Reset();

            if (seed == null)
                seed = TMath.Seed();

            Seed = seed.Value;

            GlobalRandom.DefaultRNG = new XorShift128Generator(seed.Value);

            Logger.Debug("Generating game world");

            var generator = new Generator(MapWidth, MapHeight);

            var fillProbability = GlobalRandom.DefaultRNG.NextUInt(40, 60);
            var cutoffBigAreaFill = GlobalRandom.DefaultRNG.NextUInt(2, 6);

            Generator = generator.ConfigAndGenerateSafe(g => g.AddSteps(GeneratorAlgorithms.OutdoorGeneneration(fillProbability: (ushort)fillProbability, cutoffBigAreaFill: (int)cutoffBigAreaFill)));

            var wallsFloors = Generator.Context
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

            foreach (var item in wallsFloors.ToArray().OfType<Wall>().ToList())
                Walls.Add(item.ID, item);

            foreach (var item in wallsFloors.ToArray().OfType<Floor>().ToList())
                Floors.Add(item.ID, item);

            CreateMap();

            Player = GameObjectFactory.CreatePlayer()
                .PositionedAt(Map.RandomPosition(MapHelpers.EmptyPointOnFloor))
                .AddToMap(Map);

            SpawnMonster(Breed.Roach);
            SpawnMonster(Breed.Roach);
            SpawnMonster(Breed.Roach);
            SpawnMonster(Breed.Roach);
            SpawnMonster(Breed.Roach);
            SpawnMonster(Breed.Roach);
            SpawnMonster(Breed.Roach);
            SpawnMonster(Breed.Roach);
            SpawnMonster(Breed.Roach);
            SpawnMonster(Breed.Roach);
        }

        private void Reset()
        {
            Walls = new WallCollection(GameObjectFactory);
            Floors = new FloorCollection(GameObjectFactory);
            Monsters = new MonsterCollection(GameObjectFactory);
            HistoricalCommands = new CommandCollection(CommandFactory);

            GameObjectFactory.Reset();
        }

        private void CreateMap()
        {
            Debug.Assert(Floors.Any() || Walls.Any(), "Walls and/or Floors must be populated");

            Map = new Map(MapWidth, MapHeight, 1, Distance.Chebyshev);

            PopulateMapTerrain();
        }

        private void PopulateMapTerrain()
        {
            var wallsFloors = Walls.Values.Cast<Terrain>()
                .Union(Floors.Values)
                .OrderBy(t => t.CreatedIndex)
                .ToArrayView(MapWidth);

            Map.ApplyTerrainOverlay(wallsFloors);
        }

        public void MoveRequest(Direction direction)
        {
            var walkCommand = CommandFactory.CreateWalkCommand();
            walkCommand.Initialise(Player, direction);

            var results = ExecuteCommand(walkCommand);

            WriteMessages(results);
        }

        private void WriteMessages(IEnumerable<CommandResult> results)
        {
            foreach (var result in results.SelectMany(s => s.Messages).ToList())
            {
                MessageLog.AddMessage(result);
            }
        }

        private IEnumerable<CommandResult> ExecuteCommand(BaseCommand command)
        {
            var result = command.Execute();

            yield return result;

            foreach (var subsequentCommand in result.SubsequentCommands)
            {
                foreach (var subsequentResult in ExecuteCommand(subsequentCommand))
                    yield return subsequentResult;
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

        public void SpawnMonster(Breed breed)
        {
            var monster = GameObjectFactory
                .CreateMonster()
                .WithBreed(breed)
                .PositionedAt(Map.RandomPositionAwayFrom(Player.Position, 5, MapHelpers.EmptyPointOnFloor))
                .AddToMap(Map);

            Monsters.Add(monster.ID, monster);

            Mediator.Send(new MapTileChangedRequest(monster.Position));
        }

        public void SpawnMonster(string breed)
        {
            SpawnMonster(Breed.GetBreed(breed));
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
            SaveState(SaveGameStore);

            return SaveGameStore.SaveStoreToFile(saveGameName, overwrite);
        }

        public LoadGameResult LoadGame(string saveGameName)
        {
            var loadGameResult = SaveGameStore.LoadStoreFromFile(saveGameName);

            if (loadGameResult.Equals(LoadGameResult.Success))
                LoadState(SaveGameStore);

            return loadGameResult;
        }

        public void SaveState(ISaveGameStore saveGameStore)
        {
            GameObjectFactory.SaveState(saveGameStore);
            Walls.SaveState(saveGameStore);
            Floors.SaveState(saveGameStore);
            Monsters.SaveState(saveGameStore);
            MessageLog.SaveState(saveGameStore);
            Player.SaveState(saveGameStore);
            HistoricalCommands.SaveState(saveGameStore);

            var gameWorldSaveData = Memento<GameWorldSaveData>.CreateWithAutoMapper(this, saveGameStore.Mapper);
            saveGameStore.SaveToStore(gameWorldSaveData);
        }

        public void LoadState(ISaveGameStore saveGameStore)
        {
            Reset();

            var gameWorldSaveData = saveGameStore.GetFromStore<GameWorldSaveData>();
            Memento<GameWorldSaveData>.SetWithAutoMapper(this, gameWorldSaveData, saveGameStore.Mapper);
            
            GameObjectFactory.LoadState(saveGameStore);
            Walls.LoadState(saveGameStore);
            Floors.LoadState(saveGameStore);
            Monsters.LoadState(saveGameStore);
            MessageLog.LoadState(saveGameStore);

            var playerSaveData = saveGameStore.GetFromStore<PlayerSaveData>();
            Player = GameObjectFactory.CreatePlayer(playerSaveData.State.Id);
            Player.LoadState(saveGameStore);
            HistoricalCommands.LoadState(saveGameStore);

            CreateMap();

            PopulateMapTerrain();

            Map.AddEntity(Player);

            foreach (var monster in Monsters.Values)
            {
                Map.AddEntity(monster);
            }
        }

        public IMemento<GameWorldSaveData> GetSaveState(IMapper mapper)
        {
            return Memento<GameWorldSaveData>.CreateWithAutoMapper(this, mapper);
        }

        public void SetLoadState(IMemento<GameWorldSaveData> memento, IMapper mapper)
        {
            Memento<GameWorldSaveData>.SetWithAutoMapper(this, memento, mapper);
        }
    }
}