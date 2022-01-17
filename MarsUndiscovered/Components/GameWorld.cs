using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.FOV;
using MarsUndiscovered.Extensions;
using MarsUndiscovered.Interfaces;

using GoRogue.GameFramework;
using GoRogue.Pathing;
using GoRogue.Random;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Components.SaveData;

using Microsoft.Xna.Framework.Input;

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
        public IFOV FieldOfView { get; set; }
        public Player Player { get; private set; }
        public IGameObjectFactory GameObjectFactory { get; set; }
        public ICommandFactory CommandFactory { get; set; }
        public IGameTurnService GameTurnService { get; set; }
        public ISaveGameService SaveGameService { get; set; }
        public IMapGenerator MapGenerator { get; set; }
        public IMonsterGenerator MonsterGenerator { get; set; }
        public IItemGenerator ItemGenerator { get; set; }
        public WallCollection Walls { get; private set; }
        public FloorCollection Floors { get; private set; }
        public MonsterCollection Monsters { get; private set; }
        public ItemCollection Items { get; private set; }
        public CommandCollection HistoricalCommands { get; private set; }
        public IDictionary<uint, IGameObject> GameObjects => GameObjectFactory.GameObjects;
        public MessageLog MessageLog { get; } = new MessageLog();
        public IMapper Mapper { get; set; }
        public uint Seed { get; set; }

        public string LoadGameDetail
        {
            get => $"Seed: {Seed}";
            set { }
        }

        public Inventory Inventory { get; private set; }

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

            Inventory = new Inventory(this);

            //SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach));
            //SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach));
            //SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach));
            //SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach));

            SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots));

            RebuildGoalMaps();
            CreateFieldOfView();
        }

        private void CreateFieldOfView()
        {
            FieldOfView = new RecursiveShadowcastingFOV(Map.TransparencyView);
            UpdateFieldOfView();
        }

        private void Reset()
        {
            Walls = new WallCollection(GameObjectFactory);
            Floors = new FloorCollection(GameObjectFactory);
            Monsters = new MonsterCollection(GameObjectFactory);
            Items = new ItemCollection(GameObjectFactory);
            HistoricalCommands = new CommandCollection(CommandFactory, this);

            GameObjectFactory.Reset();
        }

        public void UpdateFieldOfView()
        {
            FieldOfView.Calculate(Player.Position);
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

        public IList<CommandResult> MoveRequest(Path path)
        {
            var commandResults = new List<CommandResult>();

            if (path == null)
                return commandResults;

            if (path.Length == 1)
            {
                return MoveRequest(Direction.GetDirection(Player.Position, path.GetStep(0)));
            }

            var nextPoint = path.GetStep(0);

            if (Player.Position != path.Start)
            {
                var subsequentSteps = path.Steps
                    .SkipWhile(s => s != Player.Position)
                    .Skip(1)
                    .ToList();

                if (!subsequentSteps.Any())
                    return commandResults;

                nextPoint = subsequentSteps.First();
            }

            if (Map.GetObjectAt<Wall>(nextPoint) != null)
                return commandResults;

            foreach (var surroundingPoint in AdjacencyRule.EightWay.Neighbors(Player.Position))
            {
                if (Map.Bounds().Contains(surroundingPoint) && Map.GetObjectAt<Monster>(surroundingPoint) != null)
                    return commandResults;
            }

            var result = MoveRequest(Direction.GetDirection(Player.Position, nextPoint));

            return result;
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

                GameTurnService.NextTurn();
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

        public void SpawnItem(SpawnItemParams spawnItemParams)
        {
            ItemGenerator.SpawnItem(spawnItemParams, Map, Items);
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
            Items.SaveState(saveGameService);
            MessageLog.SaveState(saveGameService);
            Player.SaveState(saveGameService);
            HistoricalCommands.SaveState(saveGameService);
            Inventory.SaveState(saveGameService);

            var gameWorldSaveData = Memento<GameWorldSaveData>.CreateWithAutoMapper(this, saveGameService.Mapper);
            saveGameService.SaveToStore(gameWorldSaveData);

            var randomNumberSaveData = GetRandomNumberSaveData();

            saveGameService.SaveToStore(new Memento<RandomNumberSaveData>(randomNumberSaveData));
        }

        private RandomNumberSaveData GetRandomNumberSaveData()
        {
            var xorShift128Generator = (XorShift128Generator)GlobalRandom.DefaultRNG;

            var xorShift128GeneratorType = typeof(XorShift128Generator);
            var abstractGeneratorType = typeof(AbstractGenerator);

            var bytesAvailableFieldBool = xorShift128GeneratorType.GetField("_bytesAvailable", BindingFlags.NonPublic | BindingFlags.Instance);
            var xFieldULong = xorShift128GeneratorType.GetField("_x", BindingFlags.NonPublic | BindingFlags.Instance);
            var yFieldULong = xorShift128GeneratorType.GetField("_y", BindingFlags.NonPublic | BindingFlags.Instance);
            var bitBufferFieldUInt = abstractGeneratorType.GetField("_bitBuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            var bitCountFieldInt = abstractGeneratorType.GetField("_bitCount", BindingFlags.NonPublic | BindingFlags.Instance);

            var randomNumberSaveData = new RandomNumberSaveData
            {
                BytesAvailable = (bool)bytesAvailableFieldBool.GetValue(xorShift128Generator),
                X = (ulong)xFieldULong.GetValue(xorShift128Generator),
                Y = (ulong)yFieldULong.GetValue(xorShift128Generator),
                BitBuffer = (uint)bitBufferFieldUInt.GetValue(xorShift128Generator),
                BitCount = (int)bitCountFieldInt.GetValue(xorShift128Generator),
                Seed = xorShift128Generator.Seed
            };
            return randomNumberSaveData;
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
            Items.LoadState(saveGameService);
            MessageLog.LoadState(saveGameService);

            var playerSaveData = saveGameService.GetFromStore<PlayerSaveData>();
            Player = GameObjectFactory.CreatePlayer(playerSaveData.State.Id);
            Player.LoadState(saveGameService);
            HistoricalCommands.LoadState(saveGameService);

            Map = MapGenerator.CreateMap(Walls, Floors);

            Map.AddEntity(Player);

            foreach (var monster in Monsters.Values)
            {
                if (monster.Position != Point.None)
                    Map.AddEntity(monster);
            }

            foreach (var item in Items.Values)
            {
                if (item.Position != Point.None)
                    Map.AddEntity(item);
            }

            Inventory = new Inventory(this);
            Inventory.LoadState(saveGameService);

            LoadRandomNumberSaveData(saveGameService);
            CreateFieldOfView();
        }

        private void LoadRandomNumberSaveData(ISaveGameService saveGameService)
        {
            var randomNumberSaveData = saveGameService.GetFromStore<RandomNumberSaveData>();

            var xorShift128Generator = (XorShift128Generator)GlobalRandom.DefaultRNG;

            xorShift128Generator.Seed = randomNumberSaveData.State.Seed;

            var xorShift128GeneratorType = typeof(XorShift128Generator);
            var abstractGeneratorType = typeof(AbstractGenerator);

            var bytesAvailableFieldBool = xorShift128GeneratorType.GetField("_bytesAvailable", BindingFlags.NonPublic | BindingFlags.Instance);
            var xFieldULong = xorShift128GeneratorType.GetField("_x", BindingFlags.NonPublic | BindingFlags.Instance);
            var yFieldULong = xorShift128GeneratorType.GetField("_y", BindingFlags.NonPublic | BindingFlags.Instance);
            var bitBufferFieldUInt = abstractGeneratorType.GetField("_bitBuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            var bitCountFieldInt = abstractGeneratorType.GetField("_bitCount", BindingFlags.NonPublic | BindingFlags.Instance);

            bytesAvailableFieldBool.SetValue(xorShift128Generator, randomNumberSaveData.State.BytesAvailable);
            xFieldULong.SetValue(xorShift128Generator, randomNumberSaveData.State.X);
            yFieldULong.SetValue(xorShift128Generator, randomNumberSaveData.State.Y);
            bitBufferFieldUInt.SetValue(xorShift128Generator, randomNumberSaveData.State.BitBuffer);
            bitCountFieldInt.SetValue(xorShift128Generator, randomNumberSaveData.State.BitCount);
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

        public Path GetPathToPlayer(Point mapPosition)
        {
            if (!Map.Bounds().Contains(mapPosition))
                return null;

            return Map.AStar.ShortestPath(Player.Position, mapPosition);
        }

        public string GetGameObjectInformationAt(Point point)
        {
            if (!Map.Bounds().Contains(point))
                return null;

            var monster = Map.GetObjectAt<Monster>(point);

            if (monster != null)
            {
                return monster.GetInformation(Player);
            }

            return null;
        }

        public List<InventoryItem> GetInventoryItems()
        {
            return Inventory.GetInventoryItems();
        }

        public IList<CommandResult> DropItemRequest(Keys itemKey)
        {
            if (!Inventory.ItemKeyAssignments.TryGetValue(itemKey, out var itemGroup))
                return null;

            var dropItemCommand = CommandFactory.CreateDropItemCommand(this);

            dropItemCommand.Initialise(Player, itemGroup.First());

            return ExecuteCommand(dropItemCommand).ToList();
        }

        public IList<CommandResult> EquipItemRequest(Keys itemKey)
        {
            if (!Inventory.ItemKeyAssignments.TryGetValue(itemKey, out var itemGroup))
                return null;

            var equipItemCommand = CommandFactory.CreateEquipItemCommand(this);

            equipItemCommand.Initialise(itemGroup.First());

            return ExecuteCommand(equipItemCommand).ToList();
        }

        public IList<CommandResult> UnequipItemRequest(Keys itemKey)
        {
            if (!Inventory.ItemKeyAssignments.TryGetValue(itemKey, out var itemGroup))
                return null;

            var unequipItemCommand = CommandFactory.CreateUnequipItemCommand(this);

            unequipItemCommand.Initialise(itemGroup.First());

            return ExecuteCommand(unequipItemCommand).ToList();
        }
    }
}
