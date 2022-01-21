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
using MarsUndiscovered.Messages;

using Microsoft.Xna.Framework.Input;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

using Troschuetz.Random;
using Troschuetz.Random.Generators;

namespace MarsUndiscovered.Components
{
    public class GameWorld : BaseComponent, IGameWorld, ISaveable, IMementoState<GameWorldSaveData>
    {
        public MapCollection Maps { get; private set; }
        public MarsMap CurrentMap => Maps.CurrentMap;
        public GoalMaps GoalMaps { get; set; }
        public Player Player { get; private set; }
        public IGameObjectFactory GameObjectFactory { get; set; }
        public ICommandFactory CommandFactory { get; set; }
        public IGameTurnService GameTurnService { get; set; }
        public ISaveGameService SaveGameService { get; set; }
        public IMapGenerator MapGenerator { get; set; }
        public IMonsterGenerator MonsterGenerator { get; set; }
        public IItemGenerator ItemGenerator { get; set; }
        public IShipGenerator ShipGenerator { get; set; }
        public IMapExitGenerator MapExitGenerator { get; set; }
        public WallCollection Walls { get; private set; }
        public FloorCollection Floors { get; private set; }
        public MonsterCollection Monsters { get; private set; }
        public ItemCollection Items { get; private set; }
        public MapExitCollection MapExits { get; private set; }
        public ShipCollection Ships { get; private set; }
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

            var currentMap = CreateMap();
            Maps.CurrentMap = currentMap;
            var map2 = CreateMap();

            ShipGenerator.CreateShip(GameObjectFactory, currentMap, Ships);

            Player = GameObjectFactory
                .CreatePlayer()
                .PositionedAt(new Point(CurrentMap.Width / 2, CurrentMap.Height - 2 - (Constants.ShipOffset - 1))) // Start off underneath the ship, extra -1 for the current ship design as there's a blank space on the bottom line
                .AddToMap(CurrentMap);

            Inventory = new Inventory(this);

            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach));

            SpawnMonster(new SpawnMonsterParams().OnMap(map2.Id).WithBreed(Breed.Roach));
            SpawnMonster(new SpawnMonsterParams().OnMap(map2.Id).WithBreed(Breed.Roach));

            SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots));

            SpawnItem(new SpawnItemParams().OnMap(map2.Id).WithItemType(ItemType.MagnesiumPipe));
            SpawnItem(new SpawnItemParams().OnMap(map2.Id).WithItemType(ItemType.MagnesiumPipe));
            SpawnItem(new SpawnItemParams().OnMap(map2.Id).WithItemType(ItemType.ShieldGenerator));
            SpawnItem(new SpawnItemParams().OnMap(map2.Id).WithItemType(ItemType.ShieldGenerator));
            SpawnItem(new SpawnItemParams().OnMap(map2.Id).WithItemType(ItemType.HealingBots));
            SpawnItem(new SpawnItemParams().OnMap(map2.Id).WithItemType(ItemType.HealingBots));

            var mapExit2 = SpawnMapExit(new SpawnMapExitParams().OnMap(map2.Id).WithDirection(Direction.Up));

            if (mapExit2 != null)
            {
                var mapExit1 = SpawnMapExit(
                    new SpawnMapExitParams().OnMap(CurrentMap.Id).ToMapExit(mapExit2.ID).WithDirection(Direction.Down)
                );

                mapExit2.Destination = mapExit1;
            }

            RebuildGoalMaps();
        }

        private MarsMap CreateMap()
        {
            var map = MapGenerator.CreateMap(this, GameObjectFactory, Components.Maps.MapGenerator.CreateOutdoorWallsFloors);

            var terrain = GameObjects
                .Values
                .OfType<Terrain>()
                .Where(g => Equals(g.CurrentMap, map))
                .ToList();

            foreach (var wall in terrain.OfType<Wall>())
                Walls.Add(wall.ID, wall);

            foreach (var floor in terrain.OfType<Floor>())
                Floors.Add(floor.ID, floor);

            Maps.Add(map);

            return map;
        }

        private void Reset()
        {
            Walls = new WallCollection(GameObjectFactory);
            Floors = new FloorCollection(GameObjectFactory);
            Monsters = new MonsterCollection(GameObjectFactory);
            Items = new ItemCollection(GameObjectFactory);
            MapExits = new MapExitCollection(GameObjectFactory);
            Ships = new ShipCollection(GameObjectFactory);
            Maps = new MapCollection(this);
            HistoricalCommands = new CommandCollection(CommandFactory, this);

            GameObjectFactory.Reset();
        }

        public void UpdateFieldOfView(bool partialUpdate = true)
        {
            CurrentMap.UpdateFieldOfView(Player.Position);

            if (partialUpdate)
            {
                Mediator.Publish(
                    new FieldOfViewChangedNotifcation(
                        CurrentMap.PlayerFOV.NewlySeen,
                        CurrentMap.PlayerFOV.NewlyUnseen,
                        CurrentMap.SeenTiles
                    )
                );
            }
            else
            {
                Mediator.Publish(
                    new FieldOfViewChangedNotifcation(
                        CurrentMap.PlayerFOV.CurrentFOV,
                        CurrentMap.Positions(),
                        CurrentMap.SeenTiles
                    )
                );
            }
        }

        public void AfterCreateGame()
        {
            CurrentMap.UpdateSeenTiles(CurrentMap.PlayerFOV.CurrentFOV);

            Mediator.Publish(new FieldOfViewChangedNotifcation(CurrentMap.PlayerFOV.CurrentFOV, CurrentMap.Positions(), CurrentMap.SeenTiles));
        }

        public void ChangeMap(MarsMap map)
        {
            Maps.CurrentMap = map;
            Mediator.Publish(new MapChangedNotification());
            UpdateFieldOfView(false);
            RebuildGoalMaps();
        }

        public void RebuildGoalMaps()
        {
            GoalMaps.Rebuild(CurrentMap);
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

            if (CurrentMap.GetObjectAt<Wall>(nextPoint) != null)
                return commandResults;

            foreach (var surroundingPoint in AdjacencyRule.EightWay.Neighbors(Player.Position))
            {
                if (CurrentMap.Bounds().Contains(surroundingPoint) && CurrentMap.GetObjectAt<Monster>(surroundingPoint) != null)
                    return commandResults;
            }

            var result = MoveRequest(Direction.GetDirection(Player.Position, nextPoint));

            return result;
        }

        public IEnumerable<CommandResult> NextTurn()
        {
            foreach (var monster in Monsters.LiveMonsters.Where(m => m.CurrentMap.Equals(CurrentMap)))
            {
                if (Player.IsDead)
                    yield break;

                var direction = GoalMaps.GoalMap.GetDirectionOfMinValue(monster.Position, AdjacencyRule.EightWay);

                if (direction != Direction.None)
                {
                    var positionBefore = monster.Position;

                    var positionAfter = monster.Position.Add(direction);

                    var player = CurrentMap.GetObjectAt<Player>(positionAfter);

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
            var map = spawnMonsterParams.MapId.HasValue ? Maps.First(m => m.Id == spawnMonsterParams.MapId) : CurrentMap;

            MonsterGenerator.SpawnMonster(spawnMonsterParams, GameObjectFactory, map, Monsters);
        }

        public void SpawnItem(SpawnItemParams spawnItemParams)
        {
            var map = spawnItemParams.MapId.HasValue ? Maps.First(m => m.Id == spawnItemParams.MapId) : CurrentMap;

            ItemGenerator.SpawnItem(spawnItemParams, GameObjectFactory, map, Items);
        }

        private MapExit SpawnMapExit(SpawnMapExitParams spawnMapExitParams)
        {
            var map = spawnMapExitParams.MapId.HasValue ? Maps.First(m => m.Id == spawnMapExitParams.MapId) : CurrentMap;

            return MapExitGenerator.SpawnMapExit(spawnMapExitParams, GameObjectFactory, map, MapExits);
        }

        public void CreateWall(Point position)
        {
            CurrentMap.CreateWall(position, GameObjectFactory);
            UpdateFieldOfView();
        }

        public void CreateFloor(Point position)
        {
            CurrentMap.CreateFloor(position, GameObjectFactory);
            UpdateFieldOfView();
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
            MapExits.LoadState(saveGameService);
            Ships.LoadState(saveGameService);
            MessageLog.LoadState(saveGameService);

            var playerSaveData = saveGameService.GetFromStore<PlayerSaveData>();
            Player = GameObjectFactory.CreatePlayer(playerSaveData.State.Id);
            Player.LoadState(saveGameService);
            HistoricalCommands.LoadState(saveGameService);
            Maps.LoadState(saveGameService);
            Inventory = new Inventory(this);
            Inventory.LoadState(saveGameService);

            GlobalRandom.DefaultRNG = saveGameService.GetFromStore<XorShift128Generator>().State;
        }

        public void SaveState(ISaveGameService saveGameService)
        {
            GameObjectFactory.SaveState(saveGameService);
            Walls.SaveState(saveGameService);
            Floors.SaveState(saveGameService);
            Monsters.SaveState(saveGameService);
            Items.SaveState(saveGameService);
            MapExits.SaveState(saveGameService);
            Ships.SaveState(saveGameService);
            MessageLog.SaveState(saveGameService);
            Player.SaveState(saveGameService);
            HistoricalCommands.SaveState(saveGameService);
            Inventory.SaveState(saveGameService);
            Maps.SaveState(saveGameService);

            var gameWorldSaveData = Memento<GameWorldSaveData>.CreateWithAutoMapper(this, saveGameService.Mapper);
            saveGameService.SaveToStore(gameWorldSaveData);

            // Not sure I like this idea, as a memento is supposed to hold state, not a reference to an object. It does make the code more straightforward for saving and loading
            // though.
            saveGameService.SaveToStore(new Memento<XorShift128Generator>((XorShift128Generator)GlobalRandom.DefaultRNG));
        }

        public IMemento<GameWorldSaveData> GetSaveState(IMapper mapper)
        {
            return Memento<GameWorldSaveData>.CreateWithAutoMapper(this, mapper);
        }

        public void SetLoadState(IMemento<GameWorldSaveData> memento, IMapper mapper)
        {
            Memento<GameWorldSaveData>.SetWithAutoMapper(this, memento, mapper);
        }

        public PlayerStatus GetPlayerStatus()
        {
            return Mapper.Map<PlayerStatus>(Player);
        }

        public IList<MonsterStatus> GetStatusOfMonstersInView()
        {
            var status = Monsters.LiveMonsters
                .Where(m => m.CurrentMap.Equals(CurrentMap))
                .Where(m => CurrentMap.PlayerFOV.BooleanResultView[m.Position])
                .Select(
                    m =>
                    {
                        var monster = Mapper.Map<MonsterStatus>(m);

                        monster.DistanceFromPlayer = CurrentMap.DistanceMeasurement.Calculate(m.Position, Player.Position);

                        return monster;
                    }
                )
                .ToList();

            return status;
        }

        public Path GetPathToPlayer(Point mapPosition)
        {
            if (!CurrentMap.Bounds().Contains(mapPosition))
                return null;

            return CurrentMap.AStar.ShortestPath(Player.Position, mapPosition);
        }

        public string GetGameObjectInformationAt(Point point)
        {
            if (!CurrentMap.Bounds().Contains(point))
                return null;

            var monster = CurrentMap.GetObjectAt<Monster>(point);

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
