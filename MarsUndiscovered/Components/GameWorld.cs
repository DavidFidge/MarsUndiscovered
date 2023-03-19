using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Extensions;
using MarsUndiscovered.Interfaces;

using GoRogue.GameFramework;
using GoRogue.Pathing;
using GoRogue.Random;
using MarsUndiscovered.Components.Dto;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Messages;

using Microsoft.Xna.Framework.Input;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Components
{
    public class GameWorld : BaseComponent, IGameWorld, ISaveable
    {
        public Guid GameId { get; private set; }
        public Player Player { get; private set; }
        public IMorgue Morgue { get; set; }
        public IGameObjectFactory GameObjectFactory { get; set; }
        public IGameTimeService GameTimeService { get; set; }
        public ICommandFactory CommandFactory { get; set; }
        public IGameTurnService GameTurnService { get; set; }
        public ISaveGameService SaveGameService { get; set; }
        public IMapGenerator MapGenerator { get; set; }
        public IMonsterGenerator MonsterGenerator { get; set; }
        public IItemGenerator ItemGenerator { get; set; }
        public IShipGenerator ShipGenerator { get; set; }
        public IMiningFacilityGenerator MiningFacilityGenerator { get; set; }
        public IMapExitGenerator MapExitGenerator { get; set; }
        public WallCollection Walls { get; private set; }
        public FloorCollection Floors { get; private set; }
        public MonsterCollection Monsters { get; private set; }
        public ItemCollection Items { get; private set; }
        public MapExitCollection MapExits { get; private set; }
        public ShipCollection Ships { get; private set; }
        public MiningFacilityCollection MiningFacilities { get; private set; }
        public CommandCollection HistoricalCommands { get; private set; }
        public IDictionary<uint, IGameObject> GameObjects => GameObjectFactory.GameObjects;

        public MapCollection Maps { get; private set; }
        public MarsMap CurrentMap => Maps.CurrentMap;

        private readonly MessageLog _messageLog = new();
        private readonly RadioComms _radioComms = new();
        
        public ulong Seed { get; set; }
        protected IList<Monster> MonstersInView = new List<Monster>();
        protected IList<Monster> LastMonstersInView = new List<Monster>();

        public string LoadGameDetail
        {
            get => $"Seed: {Seed}";
            set { }
        }

        public Inventory Inventory { get; private set; }

        private BaseGameActionCommand[] _replayHistoricalCommands;
        private int _replayHistoricalCommandIndex;

        private AutoExploreGoalMap _autoExploreGoalMap;

        public GameWorld()
        {
            GlobalRandom.DefaultRNG = new MizuchiRandom();
        }
        
        private ulong MakeSeed()
        {
            unchecked
            {
                var seedingRandom = new System.Random();
                return (ulong)seedingRandom.Next() ^ (ulong)seedingRandom.Next() << 21 ^ (ulong)seedingRandom.Next() << 42;
            }
        }

        public void NewGame(ulong? seed = null)
        {
            Reset();
            Morgue.GameStarted();
            
            seed ??= MakeSeed();

            Seed = seed.Value;

            GlobalRandom.DefaultRNG = new MizuchiRandom(seed.Value);

            Logger.Debug("Generating game world");

            var mapLevel1 = CreateLevel1();
            Maps.CurrentMap = mapLevel1;

            CreateLevel2(mapLevel1);
            
            Inventory = new Inventory(this);
            
            _radioComms.CreateGameStartMessages(Ships.First().Value, _messageLog);

            ResetFieldOfView();
            GameTimeService.Start();
        }
        
        // Currently used for unit tests. Level generation could be factored out to strategies in the future, then this
        // may be able to be changed.
        public void NewBlankGame(ulong? seed = null)
        {
            Reset();
            Morgue.GameStarted();
            
            seed ??= MakeSeed();

            Seed = seed.Value;

            GlobalRandom.DefaultRNG = new MizuchiRandom(seed.Value);

            Logger.Debug("Generating game world");
            
            MapGenerator.CreateOutdoorMap(this, GameObjectFactory);
            AddMapToGame(MapGenerator.MarsMap);
            var map = MapGenerator.MarsMap;
            
            Player = GameObjectFactory
                .CreatePlayer()
                .PositionedAt(new Point(map.Width / 2,
                    map.Height - 2 -
                    (Constants.ShipOffset -
                     1))) // Start off underneath the ship, extra -1 for the current ship design as there's a blank space on the bottom line
                .AddToMap(map);
            
            Maps.CurrentMap = map;
            
            Inventory = new Inventory(this);

            ResetFieldOfView();
            GameTimeService.Start();
        }

        private MarsMap CreateLevel2(MarsMap mapLevel1)
        {
            MapGenerator.CreateOutdoorMap(this, GameObjectFactory);
            AddMapToGame(MapGenerator.MarsMap);
            var mapLevel2 = MapGenerator.MarsMap;

            SpawnMonster(new SpawnMonsterParams().OnMap(mapLevel2.Id).WithBreed("Roach"));
            SpawnMonster(new SpawnMonsterParams().OnMap(mapLevel2.Id).WithBreed("Repair Drone"));
            SpawnMonster(new SpawnMonsterParams().OnMap(mapLevel2.Id).WithBreed("Tesla Coil"));

            SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.MagnesiumPipe));
            SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.MagnesiumPipe));
            SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.IronSpike));
            SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.IronSpike));
            SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.ShieldGenerator));
            SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.ShieldGenerator));
            SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.HealingBots));
            SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.HealingBots));
            SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.ShipRepairParts));

            var mapExit2 = SpawnMapExit(new SpawnMapExitParams().OnMap(mapLevel2.Id).WithDirection(Direction.Up));

            if (mapExit2 != null)
            {
                var miningFacilityPointsOnMap1 = MiningFacilities.Values
                    .Where(m => ((MarsMap)m.CurrentMap).Id == mapLevel1.Id)
                    .Select(m => m.Position)
                    .GroupBy(m => m.Y)
                    .MaxBy(m => m.Key)
                    .ToList();
                
                var mapExit1 = SpawnMapExit(
                    new SpawnMapExitParams()
                        .OnMap(mapLevel1.Id)
                        .ToMapExit(mapExit2.ID)
                        .WithDirection(Direction.Down)
                        .AtFreeSpotNextTo(mapLevel1, miningFacilityPointsOnMap1)
                );

                mapExit2.Destination = mapExit1;
            }

            return mapLevel2;
        }

        private MarsMap CreateLevel1()
        {
            MapGenerator.CreateOutdoorMap(this, GameObjectFactory);
            AddMapToGame(MapGenerator.MarsMap);
            var map = MapGenerator.MarsMap;
            
            ShipGenerator.CreateShip(GameObjectFactory, map, Ships);
            MiningFacilityGenerator.CreateMiningFacility(GameObjectFactory, map, MiningFacilities);
            
            Player = GameObjectFactory
                .CreatePlayer()
                .PositionedAt(new Point(map.Width / 2,
                    map.Height - 2 -
                    (Constants.ShipOffset -
                     1))) // Start off underneath the ship, extra -1 for the current ship design as there's a blank space on the bottom line
                .AddToMap(map);
            
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
            
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).OnMap(map.Id));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).OnMap(map.Id));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.IronSpike).OnMap(map.Id));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.IronSpike).OnMap(map.Id));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator).OnMap(map.Id));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator).OnMap(map.Id));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots).OnMap(map.Id));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots).OnMap(map.Id));

            return map;
        }

        public ProgressiveWorldGenerationResult ProgressiveWorldGeneration(ulong? seed, int step, WorldGenerationTypeParams worldGenerationTypeParams)
        {
            Reset();
            
            seed ??= MakeSeed();

            Seed = seed.Value;

            GlobalRandom.DefaultRNG = new MizuchiRandom(seed.Value);

            Logger.Debug("Generating world in world builder");

            switch (worldGenerationTypeParams.MapType)
            {
                case MapType.Outdoor:
                    MapGenerator.CreateOutdoorMap(this, GameObjectFactory, step);
                    break;
                case MapType.Mine:
                    MapGenerator.CreateMineMap(this, GameObjectFactory, step);
                    break;
            }
            
            AddMapToGame(MapGenerator.MarsMap);

            Maps.CurrentMap = MapGenerator.MarsMap;

            if (!MapGenerator.IsComplete || step <= MapGenerator.Steps)
                return new ProgressiveWorldGenerationResult { Seed = Seed, IsFinalStep = false};

            Player = GameObjectFactory
                .CreatePlayer()
                .PositionedAt(GlobalRandom.DefaultRNG.RandomPosition(CurrentMap, MapHelpers.EmptyPointOnFloor))
                .AddToMap(CurrentMap);

            Inventory = new Inventory(this);

            SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")));
            SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            
            return new ProgressiveWorldGenerationResult { Seed = Seed, IsFinalStep = true };
        }
        
        protected void ResetFieldOfView()
        {
            CurrentMap.ResetFieldOfView();
            UpdateFieldOfView(false);
            UpdateMonstersInView();
            LastMonstersInView = MonstersInView;
        }

        private MarsMap AddMapToGame(MarsMap marsMap)
        {
            var terrain = GameObjects
                .Values
                .OfType<Terrain>()
                .Where(g => Equals(g.CurrentMap, MapGenerator.MarsMap))
                .ToList();

            foreach (var wall in terrain.OfType<Wall>())
                Walls.Add(wall.ID, wall);

            foreach (var floor in terrain.OfType<Floor>())
                Floors.Add(floor.ID, floor);

            Maps.Add(MapGenerator.MarsMap);

            return MapGenerator.MarsMap;
        }

        private void Reset()
        {
            GameId = Guid.NewGuid();
            GameTimeService.Reset();
            Morgue.Reset();
            Walls = new WallCollection(GameObjectFactory);
            Floors = new FloorCollection(GameObjectFactory);
            Monsters = new MonsterCollection(GameObjectFactory);
            Items = new ItemCollection(GameObjectFactory);
            MapExits = new MapExitCollection(GameObjectFactory);
            Ships = new ShipCollection(GameObjectFactory);
            MiningFacilities = new MiningFacilityCollection(GameObjectFactory);
            Maps = new MapCollection();
            HistoricalCommands = new CommandCollection(CommandFactory, this);
            _autoExploreGoalMap = new AutoExploreGoalMap();

            GameObjectFactory.Initialise(this);
        }

        public void UpdateFieldOfView(bool partialUpdate = true)
        {
            CurrentMap.UpdateFieldOfView(Player.Position, Player.VisualRange);

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
                        CurrentMap.Positions().ToEnumerable(),
                        CurrentMap.SeenTiles
                    )
                );
            }
        }

        public void AfterCreateGame()
        {
            UpdateFieldOfView(false);
            CurrentMap.UpdateSeenTiles(CurrentMap.PlayerFOV.CurrentFOV);

            Mediator.Publish(new FieldOfViewChangedNotifcation(CurrentMap.PlayerFOV.CurrentFOV, CurrentMap.Positions().ToEnumerable(), CurrentMap.SeenTiles));
        }
        
        public void AfterProgressiveWorldGeneration()
        {
            // Show all monsters and all of map
            MonstersInView = Monsters.LiveMonsters.ToList();
            Mediator.Publish(new MapChangedNotification());

            Mediator.Publish(
                new FieldOfViewChangedNotifcation(
                    CurrentMap.Positions().ToEnumerable(),
                    Array.Empty<Point>(),
                    new ArrayView<SeenTile>(CurrentMap.Width, CurrentMap.Height)
                )
            );
        }

        public async Task WriteMorgueToFile(Guid gameId)
        {
            await Morgue.WriteMorgueToFile(gameId);
        }

        public async Task SendMorgueToWeb(Guid gameId)
        { 
            await Morgue.SendMorgueToWeb(gameId);
        }

        public void SnapshotMorgue(string username)
        {
            Morgue.SnapshotMorgueExportData(this, username);
        }

        public void ChangeMap(MarsMap map)
        {
            Maps.CurrentMap = map;
            Mediator.Publish(new MapChangedNotification());
            UpdateFieldOfView(false);
        }

        public AutoExploreResult AutoExploreRequest(bool fallbackToMapExit = true)
        {
            _autoExploreGoalMap.Rebuild(this, fallbackToMapExit);

            var walkDirection = _autoExploreGoalMap.GoalMap.GetDirectionOfMinValue(Player.Position, AdjacencyRule.EightWay, false);

            IList<CommandResult> moveRequestResults = null;

            if (walkDirection != Direction.None)
            {
                moveRequestResults = MoveRequest(walkDirection);
            }

            // Rebuild the goal map to calculate the path for the next auto explore movement for display on the front end
            _autoExploreGoalMap.Rebuild(this, fallbackToMapExit);

            return new AutoExploreResult(_autoExploreGoalMap.GoalMap, Player, moveRequestResults, LastMonstersInView, MonstersInView);
        }

        public IList<IGameObject> GetLastSeenGameObjectsAtPosition(Point point)
        {
            return CurrentMap.LastSeenGameObjectsAtPosition(point).ToList();
        }

        public IList<IGameObject> GetObjectsAt(Point point)
        {
            return CurrentMap.GetObjectsAt(point).ToList();
        }

        public Point GetPlayerPosition()
        {
            return Player.Position;
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

            foreach (var surroundingPoint in AdjacencyRule.EightWay.Neighbors(Player.Position))
            {
                if (CurrentMap.Bounds().Contains(surroundingPoint) && CurrentMap.GetObjectAt<Monster>(surroundingPoint) != null)
                    return commandResults;
            }

            var result = MoveRequest(Direction.GetDirection(Player.Position, nextPoint)).ToList();

            return result;
        }

        protected IEnumerable<CommandResult> NextTurn()
        {
            LastMonstersInView = MonstersInView;

            foreach (var monster in Monsters.LiveMonsters.Where(m => m.CurrentMap.Equals(CurrentMap)))
            {
                if (Player.IsDead)
                    yield break;

                foreach (var command in monster.NextTurn(CommandFactory))
                {
                    foreach (var result in ExecuteCommand(command, false))
                        yield return result;
                }
            }

            Regenerate();
            RechargeItems();
            UpdateMonstersInView();
        }

        private void RechargeItems()
        {
            var rechargedItems = Items
                .RechargeItems()
                .Select(i =>
                    $"{i.GetDescriptionWithoutPrefix(Inventory.ItemTypeDiscoveries[i.ItemType])} has recharged")
                .ToList();

            _messageLog.AddMessages(rechargedItems);
        }

        protected void UpdateMonstersInView()
        {
            MonstersInView = Monsters.LiveMonsters
                .Where(m => m.CurrentMap.Equals(CurrentMap))
                .Where(m => CurrentMap.PlayerFOV.BooleanResultView[m.Position])
                .ToList();
        }

        private IEnumerable<CommandResult> ExecuteCommand(BaseGameActionCommand command, bool isPlayerAction = true)
        {
            var result = command.Execute();
            _messageLog.AddMessages(result.Messages);
            _radioComms.ProcessCommand(command, _messageLog);

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
                UpdateFieldOfView();

                foreach (var nextTurnResult in NextTurn())
                    yield return nextTurnResult;

                GameTurnService.NextTurn();
            }
        }

        public IList<string> GetMessagesSince(int currentCount)
        {
            if (currentCount == _messageLog.Count)
                return Array.Empty<string>();

            return _messageLog
                .Skip(currentCount)
                .Select(s => s.Message)
                .ToList();
        }
        
        public IList<RadioCommsItem> GetNewRadioCommsItems()
        {
            return _radioComms
                .GetNewRadioComms()
                .Select(s => new RadioCommsItem(s))
                .ToList();;
        }

        public IGridView<double?> GetGoalMap()
        {
            return Monsters.First().Value.GetGoalMap();
        }

        public void SpawnMonster(SpawnMonsterParams spawnMonsterParams)
        {
            var map = spawnMonsterParams.MapId.HasValue ? Maps.First(m => m.Id == spawnMonsterParams.MapId) : CurrentMap;

            MonsterGenerator.SpawnMonster(spawnMonsterParams, GameObjectFactory, map, Monsters);
        }

        public void SpawnItem(SpawnItemParams spawnItemParams)
        {
            var map = spawnItemParams.MapId.HasValue ? Maps.First(m => m.Id == spawnItemParams.MapId) : CurrentMap;
            
            if (spawnItemParams.IntoPlayerInventory)
                spawnItemParams.Inventory = Inventory;

            ItemGenerator.SpawnItem(spawnItemParams, GameObjectFactory, map, Items);
        }

        public MapExit SpawnMapExit(SpawnMapExitParams spawnMapExitParams)
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
            SaveState(SaveGameService, this);

            return SaveGameService.SaveStoreToFile(saveGameName, overwrite);
        }

        public LoadGameResult LoadGame(string saveGameName)
        {
            var loadGameResult = SaveGameService.LoadStoreFromFile(saveGameName);

            if (loadGameResult.Success)
                LoadState(SaveGameService, this);

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

                commands.LoadState(SaveGameService, this);

                _replayHistoricalCommands = commands
                    .OrderBy(c => c.TurnDetails.SequenceNumber)
                    .ToArray();

                _replayHistoricalCommandIndex = 0;
            }

            return loadGameResult;
        }

        bool IGameWorld.ExecuteNextReplayCommand()
        {
            return ExecuteNextReplayCommand();
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

        public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            Reset();

            GameObjectFactory.LoadState(saveGameService, gameWorld);
            Walls.LoadState(saveGameService, gameWorld);
            Floors.LoadState(saveGameService, gameWorld);
            Monsters.LoadState(saveGameService, gameWorld);
            Items.LoadState(saveGameService, gameWorld);
            MapExits.LoadState(saveGameService, gameWorld);
            Ships.LoadState(saveGameService, gameWorld);
            MiningFacilities.LoadState(saveGameService, gameWorld);
            _messageLog.LoadState(saveGameService, gameWorld);
            _radioComms.LoadState(saveGameService, gameWorld);

            var playerSaveData = saveGameService.GetFromStore<PlayerSaveData>();
            
            // Inventory must be loaded before player as player recalculates attacks based on inventory
            Inventory = new Inventory(this);
            Inventory.LoadState(saveGameService, gameWorld);

            Player = GameObjectFactory.CreatePlayer(playerSaveData.State.Id);
            Player.LoadState(saveGameService, gameWorld);
            
            Maps.LoadState(saveGameService, gameWorld);
            GameTimeService.LoadState(saveGameService);
            
            HistoricalCommands.LoadState(saveGameService, gameWorld);
            
            var gameWorldSaveData = saveGameService.GetFromStore<GameWorldSaveData>();
            SetLoadState(gameWorldSaveData);
            GameTimeService.Start();
        }

        public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            GameTimeService.Stop();
            GameObjectFactory.SaveState(saveGameService, gameWorld);
            Walls.SaveState(saveGameService, gameWorld);
            Floors.SaveState(saveGameService, gameWorld);
            Monsters.SaveState(saveGameService, gameWorld);
            Items.SaveState(saveGameService, gameWorld);
            MapExits.SaveState(saveGameService, gameWorld);
            Ships.SaveState(saveGameService, gameWorld);
            MiningFacilities.SaveState(saveGameService, gameWorld);
            _messageLog.SaveState(saveGameService, gameWorld);
            _radioComms.SaveState(saveGameService, gameWorld);
            Player.SaveState(saveGameService, gameWorld);
            HistoricalCommands.SaveState(saveGameService, gameWorld);
            Inventory.SaveState(saveGameService, gameWorld);
            Maps.SaveState(saveGameService, gameWorld);
            GameTimeService.SaveState(saveGameService);

            var gameWorldSaveData = GetSaveState();
            saveGameService.SaveToStore(gameWorldSaveData);
        }

        public IMemento<GameWorldSaveData> GetSaveState()
        {
            var memento = new Memento<GameWorldSaveData>(new GameWorldSaveData());
            memento.State.GameId = GameId;
            memento.State.Seed = Seed;
            memento.State.LoadGameDetail = LoadGameDetail;
            memento.State.RandomNumberGenerator = new MizuchiRandom(((MizuchiRandom)GlobalRandom.DefaultRNG).StateA, ((MizuchiRandom)GlobalRandom.DefaultRNG).StateB); ;
            memento.State.MonstersInView = MonstersInView.Select(m => m.ID).ToList();
            memento.State.LastMonstersInView = LastMonstersInView.Select(m => m.ID).ToList();
            return memento;
        }

        public void SetLoadState(IMemento<GameWorldSaveData> memento)
        {
            GameId = memento.State.GameId;
            Seed = memento.State.Seed;
            LoadGameDetail = memento.State.LoadGameDetail;
            LastMonstersInView = memento.State.LastMonstersInView.Select(m => Monsters[m]).ToList();
            MonstersInView = memento.State.MonstersInView.Select(m => Monsters[m]).ToList();
            GlobalRandom.DefaultRNG = new MizuchiRandom(memento.State.RandomNumberGenerator.StateA, memento.State.RandomNumberGenerator.StateB);
        }

        public PlayerStatus GetPlayerStatus()
        {
            return new PlayerStatus
            {
                Health = Player.Health,
                IsDead = Player.IsDead,
                IsVictorious = Player.IsVictorious,
                MaxHealth = Player.MaxHealth,
                Shield = Player.Shield,
                Name = Player.Name
            };
        }

        public IList<MonsterStatus> GetStatusOfMonstersInView()
        {
            var status = MonstersInView
                .Select(
                    m =>
                    {
                        var monsterStatus = new MonsterStatus
                        {
                            ID = m.ID,
                            DistanceFromPlayer = CurrentMap.DistanceMeasurement.Calculate(m.Position, Player.Position),
                            Health = m.Health,
                            MaxHealth = m.MaxHealth,
                            Name = m.Name
                        };

                        return monsterStatus;
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
        
        public IList<CommandResult> ApplyItemRequest(Keys itemKey)
        {
            if (!Inventory.ItemKeyAssignments.TryGetValue(itemKey, out var itemGroup))
                return null;

            var applyItemCommand = CommandFactory.CreateApplyItemCommand(this);

            applyItemCommand.Initialise(Player, itemGroup.First());

            return ExecuteCommand(applyItemCommand).ToList();
        }

        public void Regenerate()
        {
            Player.Regenerate();
            
            foreach (var monster in Monsters.Values.Where(m => !m.IsDead))
            {
                monster.Regenerate();
            }
        }
    }
}
