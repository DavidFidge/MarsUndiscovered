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
using GoRogue.Pathing;
using GoRogue.Random;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Components.SaveData;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

using Troschuetz.Random;
using Troschuetz.Random.Generators;

namespace MarsUndiscovered.Components
{
    public class GameWorld : BaseComponent, IGameWorld, ISaveable, IMementoState<GameWorldSaveData>
    {
        public Map Map { get; private set; }
        public GoalMaps GoalMaps { get; set; }
        public Player Player { get; private set; }
        public IGameObjectFactory GameObjectFactory { get; set; }
        public ICommandFactory CommandFactory { get; set; }
        public IGameTurnService GameTurnService { get; set; }
        public ISaveGameService SaveGameService { get; set; }
        public IMapGenerator MapGenerator { get; set; }
        public IMonsterGenerator MonsterGenerator { get; set; }
        public WallCollection Walls { get; private set; }
        public FloorCollection Floors { get; private set; }
        public MonsterCollection Monsters { get; private set; }
        public CommandCollection HistoricalCommands { get; private set; }
        public IDictionary<uint, IGameObject> GameObjects => GameObjectFactory.GameObjects;
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

        private BaseGameActionCommand[] _replayHistoricalCommands;
        private int _replayHistoricalCommandIndex;

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

            var wallsFloors = MapGenerator.CreateOutdoorWallsFloors();

            foreach (var item in wallsFloors.ToArray().OfType<Wall>().ToList())
                Walls.Add(item.ID, item);

            foreach (var item in wallsFloors.ToArray().OfType<Floor>().ToList())
                Floors.Add(item.ID, item);

            Map = MapGenerator.CreateMap(Walls, Floors);

            Player = GameObjectFactory.CreatePlayer()
                .PositionedAt(Map.RandomPosition(MapHelpers.EmptyPointOnFloor))
                .AddToMap(Map);

            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach));

            RebuildGoalMaps();
        }

        private void Reset()
        {
            Walls = new WallCollection(GameObjectFactory);
            Floors = new FloorCollection(GameObjectFactory);
            Monsters = new MonsterCollection(GameObjectFactory);
            HistoricalCommands = new CommandCollection(CommandFactory, this);

            GameObjectFactory.Reset();
        }

        public void RebuildGoalMaps()
        {
            GoalMaps.Rebuild(this);
        }

        public IList<CommandResult> MoveRequest(Direction direction)
        {
            var walkCommand = CommandFactory.CreateWalkCommand(this);
            walkCommand.Initialise(Player, direction);

            return ExecuteCommand(walkCommand).ToList();
        }

        public IEnumerable<CommandResult> NextTurn()
        {
            foreach (var monster in Monsters.LiveMonsters)
            {
                if (Player.IsDead)
                    yield break;

                var direction = GoalMaps.GoalMap.GetDirectionOfMinValue(monster.Position, AdjacencyRule.EightWay);

                if (direction != Direction.None)
                {
                    var positionBefore = monster.Position;

                    var positionAfter = monster.Position.Add(direction);

                    var player = Map.GetObjectAt<Player>(positionAfter);

                    if (player != null)
                    {
                        var attackCommand = CommandFactory.CreateAttackCommand(this);
                        attackCommand.Initialise(monster, player);

                        foreach (var result in ExecuteCommand(attackCommand, false))
                            yield return result;
                    }
                    else
                    {
                        var moveCommand = CommandFactory.CreateMoveCommand(this);
                        moveCommand.Initialise(monster, new Tuple<Point, Point>(positionBefore, positionAfter));

                        foreach (var result in ExecuteCommand(moveCommand, false))
                            yield return result;
                    }
                }
            }
        }

        private IEnumerable<CommandResult> ExecuteCommand(BaseGameActionCommand command, bool isPlayerAction = true)
        {
            var result = command.Execute();
            MessageLog.AddMessages(result.Messages);

            if (isPlayerAction)
                HistoricalCommands.AddCommand(command);

            yield return result;

            foreach (var subsequentCommand in result.SubsequentCommands)
            {
                foreach (var subsequentResult in ExecuteCommand(subsequentCommand, false))
                    yield return subsequentResult;
            }

            if (isPlayerAction && result.Result == CommandResultEnum.Success)
            {
                foreach (var nextTurnResult in NextTurn())
                    yield return nextTurnResult;
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

        public void SpawnMonster(SpawnMonsterParams spawnMonsterParams)
        {
            MonsterGenerator.SpawnMonster(spawnMonsterParams, Map, Monsters);
        }

        public SaveGameResult SaveGame(string saveGameName, bool overwrite)
        {
            if (!overwrite)
            {
                var canSaveGameResult = SaveGameService.CanSaveStoreToFile(saveGameName);
                if (canSaveGameResult.Equals(SaveGameResult.Overwrite))
                    return canSaveGameResult;
            }

            SaveGameService.Clear();
            SaveState(SaveGameService);

            return SaveGameService.SaveStoreToFile(saveGameName, overwrite);
        }

        public LoadGameResult LoadGame(string saveGameName)
        {
            var loadGameResult = SaveGameService.LoadStoreFromFile(saveGameName);

            if (loadGameResult.Success)
                LoadState(SaveGameService);

            RebuildGoalMaps();

            return loadGameResult;
        }

        public LoadGameResult LoadReplay(string saveGameName)
        {
            var loadGameResult = SaveGameService.LoadStoreFromFile(saveGameName);

            if (loadGameResult.Success)
            {
                var gameWorldSaveData = SaveGameService.GetFromStore<GameWorldSaveData>();

                NewGame(gameWorldSaveData.State.Seed);

                var commands = new CommandCollection(CommandFactory, this);

                commands.LoadState(SaveGameService);

                _replayHistoricalCommands = commands
                    .OrderBy(c => c.TurnDetails.SequenceNumber)
                    .ToArray();

                _replayHistoricalCommandIndex = 0;
            }

            RebuildGoalMaps();

            return loadGameResult;
        }

        public bool ExecuteNextReplayCommand()
        {
            if (_replayHistoricalCommandIndex < _replayHistoricalCommands.Length)
            {
                var command = _replayHistoricalCommands[_replayHistoricalCommandIndex++];
                ExecuteCommand(command).ToList();

                return true;
            }

            return false;
        }

        public void SaveState(ISaveGameService saveGameService)
        {
            GameObjectFactory.SaveState(saveGameService);
            Walls.SaveState(saveGameService);
            Floors.SaveState(saveGameService);
            Monsters.SaveState(saveGameService);
            MessageLog.SaveState(saveGameService);
            Player.SaveState(saveGameService);
            HistoricalCommands.SaveState(saveGameService);

            var gameWorldSaveData = Memento<GameWorldSaveData>.CreateWithAutoMapper(this, saveGameService.Mapper);
            saveGameService.SaveToStore(gameWorldSaveData);
        }

        public void LoadState(ISaveGameService saveGameService)
        {
            Reset();

            var gameWorldSaveData = saveGameService.GetFromStore<GameWorldSaveData>();
            Memento<GameWorldSaveData>.SetWithAutoMapper(this, gameWorldSaveData, saveGameService.Mapper);

            GameObjectFactory.LoadState(saveGameService);
            Walls.LoadState(saveGameService);
            Floors.LoadState(saveGameService);
            Monsters.LoadState(saveGameService);
            MessageLog.LoadState(saveGameService);

            var playerSaveData = saveGameService.GetFromStore<PlayerSaveData>();
            Player = GameObjectFactory.CreatePlayer(playerSaveData.State.Id);
            Player.LoadState(saveGameService);
            HistoricalCommands.LoadState(saveGameService);

            Map = MapGenerator.CreateMap(Walls, Floors);

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

        public IList<MonsterStatus> GetStatusOfMonstersInView()
        {
            var status = Monsters.LiveMonsters
                .Select(
                    m =>
                    {
                        var monster = Mapper.Map<MonsterStatus>(m);
                        monster.DistanceFromPlayer = Map.DistanceMeasurement.Calculate(m.Position, Player.Position);

                        return monster;
                    }
                )
                .ToList();

            return status;
        }

        public PlayerStatus GetPlayerStatus()
        {
            return Mapper.Map<PlayerStatus>(Player);
        }
    }
}