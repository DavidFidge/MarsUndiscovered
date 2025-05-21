using System.Text;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.GameFramework;
using GoRogue.Pathing;
using GoRogue.Random;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Dto;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Game.ViewMessages;
using MarsUndiscovered.Interfaces;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;
using Random = System.Random;

namespace MarsUndiscovered.Game.Components
{
    public class GameWorld : BaseComponent, IGameWorld, ISaveable
    {
        public ILevelGenerator LevelGenerator { get; set; }
        public Guid GameId { get; private set; }
        public Player Player { get; set; }
        public IMorgue Morgue { get; set; }
        public IStory Story { get; set; }
        public IGameObjectFactory GameObjectFactory { get; set; }
        public IGameTimeService GameTimeService { get; set; }
        public IGameTurnService GameTurnService { get; set; }
        public ISaveGameService SaveGameService { get; set; }
        public ISpawner Spawner { get; set; }
        public ICommandCollection CommandCollection { get; set; }

        public WallCollection Walls { get; private set; }
        public FloorCollection Floors { get; private set; }
        public DoorCollection Doors { get; private set; }
        public FeatureCollection Features { get; private set; }
        public MonsterCollection Monsters { get; private set; }
        public ItemCollection Items { get; private set; }
        public MachineCollection Machines { get; private set; }
        public EnvironmentalEffectCollection EnvironmentalEffects { get; private set; }
        public MapExitCollection MapExits { get; private set; }
        public ShipCollection Ships { get; private set; }

        public IDictionary<uint, IGameObject> GameObjects => GameObjectFactory.GameObjects;

        public MapCollection Maps { get; private set; }

        public MarsMap CurrentMap => Maps.CurrentMap;

        public MessageLog MessageLog { get; } = new();

        public RadioComms RadioComms { get; set; }

        public ulong Seed { get; set; }

        protected IList<Monster> MonstersInView = new List<Monster>();

        protected IList<Monster> LastMonstersInView = new List<Monster>();

        public string LoadGameDetail
        {
            get => $"Seed: {Seed}";
            set { }
        }

        public Inventory Inventory { get; private set; }
        
        private AutoExploreGoalMap _autoExploreGoalMap;
        
        public GameWorld()
        {
            GlobalRandom.DefaultRNG = new MizuchiRandom();
        }

        private ulong MakeSeed()
        {
            unchecked
            {
                var seedingRandom = new Random();
                return (ulong)seedingRandom.Next() ^ (ulong)seedingRandom.Next() << 21 ^ (ulong)seedingRandom.Next() << 42;
            }
        }

        public void NewGame(ulong? seed = null)
        {
            ResetForNewGame(seed);

            Logger.Debug("Generating game world");

            LevelGenerator.Initialise(this);
            LevelGenerator.CreateLevels();

            Inventory = new Inventory(this);

            RadioComms.CreateGameStartMessages();

            ResetFieldOfView();
        }

        private void ResetForNewGame(ulong? seed)
        {
            Reset();
            Morgue.GameStarted();

            seed ??= MakeSeed();

            Seed = seed.Value;

            GlobalRandom.DefaultRNG = new MizuchiRandom(seed.Value);

            Inventory = new Inventory(this);
            RadioComms = new RadioComms(this);
            
            GameTimeService.Reset();
            GameTimeService.Start();
        }

        public ProgressiveWorldGenerationResult ProgressiveWorldGeneration(ulong? seed, int step, WorldGenerationTypeParams worldGenerationTypeParams)
        {
            ResetForNewGame(seed);

            Logger.Debug("Generating world in world builder");

            LevelGenerator.Initialise(this);
             var result = LevelGenerator.CreateProgressive(Seed, step, worldGenerationTypeParams);

             return result;
        }

        protected void ResetFieldOfView()
        {
            CurrentMap.ResetFieldOfView();
            UpdateFieldOfView(false);
            UpdateMonstersInView();
            LastMonstersInView = MonstersInView;
        }

        public void AddMapToGame(MarsMap marsMap)
        {
            var terrain = GameObjects
                .Values
                .OfType<Terrain>()
                .Where(g => Equals(g.CurrentMap, marsMap))
                .ToList();
            
            var doors = GameObjects
                .Values
                .OfType<Door>()
                .Where(g => Equals(g.CurrentMap, marsMap))
                .ToList();
            
            var features = GameObjects
                .Values
                .OfType<Feature>()
                .Where(g => Equals(g.CurrentMap, marsMap))
                .ToList();
            
            foreach (var wall in terrain.OfType<Wall>())
                Walls.Add(wall.ID, wall);

            foreach (var floor in terrain.OfType<Floor>())
                Floors.Add(floor.ID, floor);
            
            foreach (var door in doors)
                Doors.Add(door.ID, door);
            
            foreach (var feature in features)
                Features.Add(feature.ID, feature);
            
            Maps.Add(marsMap);
        }

        private void Reset()
        {
            GameId = Guid.NewGuid();
            GameTimeService.Reset();
            Morgue.Reset();
            Walls = new WallCollection(GameObjectFactory);
            Floors = new FloorCollection(GameObjectFactory);
            Doors = new DoorCollection(GameObjectFactory);
            Features = new FeatureCollection(GameObjectFactory);
            Monsters = new MonsterCollection(GameObjectFactory);
            Items = new ItemCollection(GameObjectFactory);
            Machines = new MachineCollection(GameObjectFactory);
            EnvironmentalEffects = new EnvironmentalEffectCollection(GameObjectFactory);
            MapExits = new MapExitCollection(GameObjectFactory);
            Ships = new ShipCollection(GameObjectFactory);
            Maps = new MapCollection();
            _autoExploreGoalMap = new AutoExploreGoalMap();

            GameObjectFactory.Initialise(this);
            Spawner.Initialise(this);
            CommandCollection.Initialise();
            Story.Initialize(this);
        }

        public void UpdateFieldOfView(bool partialUpdate = true)
        {
            CurrentMap.UpdateFieldOfView(Player.Position, Player.VisualRange);

            if (partialUpdate)
            {
                Mediator.Publish(
                    new FieldOfViewChangedNotification(
                        CurrentMap.PlayerFOV.NewlySeen,
                        CurrentMap.PlayerFOV.NewlyUnseen,
                        CurrentMap.SeenTiles
                    )
                );
            }
            else
            {
                Mediator.Publish(
                    new FieldOfViewChangedNotification(
                        CurrentMap.PlayerFOV.CurrentFOV,
                        CurrentMap.Positions(),
                        CurrentMap.SeenTiles
                    )
                );
            }
        }

        public void AfterCreateGame()
        {
            UpdateFieldOfView(false);
            CurrentMap.UpdateSeenTiles(CurrentMap.PlayerFOV.CurrentFOV);

            Mediator.Publish(new FieldOfViewChangedNotification(CurrentMap.PlayerFOV.CurrentFOV, CurrentMap.Positions(), CurrentMap.SeenTiles));
        }

        public void AfterProgressiveWorldGeneration()
        {
            // Show all monsters and all of map
            MonstersInView = Monsters.LiveMonsters.ToList();
            Mediator.Publish(new MapChangedNotification());

            Mediator.Publish(
                new FieldOfViewChangedNotification(
                    CurrentMap.Positions(),
                    Array.Empty<Point>(),
                    new ArrayView<SeenTile>(CurrentMap.Width, CurrentMap.Height)
                )
            );
        }

        public async Task SendPendingMorgues()
        { 
            await Morgue.SendPendingMorgues();
        }

        public void SnapshotMorgue(string username, bool uploadMorgueFiles)
        {
            Morgue.SnapshotMorgueExportData(this, username, uploadMorgueFiles);
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
            BaseGameActionCommand command;
            
            if (direction == Direction.None)
            {
                var waitCommand = CommandCollection.CreateCommand<WaitCommand>(this);
                waitCommand.Initialise(Player);
                command = waitCommand;
            }
            else
            {
                var walkCommand = CommandCollection.CreateCommand<WalkCommand>(this);
                walkCommand.Initialise(direction);
                command = walkCommand;
            }

            return ExecuteCommand(command).ToList();
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

        public void AssignHotBarItem(Keys inventoryItemKey, Keys requestKey)
        {
            Inventory.AssignHotkey(inventoryItemKey, requestKey);
        }

        public List<InventoryItem> GetHotBarItems()
        {
            return Inventory.GetHotBarItems();
        }

        public void RemoveHotBarItem(Keys requestKey)
        {
            Inventory.RemoveHotkey(requestKey);
        }

        public IList<CommandResult> DoRangedAttack(Keys requestKey, Point target)
        {
            var item = Inventory.GetItemForKey(requestKey);

            if (item == null)
                return new List<CommandResult>();
            
            var command = CommandCollection.CreateCommand<PlayerRangeAttackCommand>(this);
            command.Initialise(target, item);
            
            return ExecuteCommand(command).ToList();
        }

        public Path GetPathForRangedAttack(Point mapPosition)
        {
            var sourcePoint = Player.Position;
            var rangeAttackPath = Lines.GetLine(sourcePoint, mapPosition).ToList();

            rangeAttackPath = rangeAttackPath
                .TakeWhile(p => p == sourcePoint || CurrentMap.GetObjectsAt(p).All(o => o.IsGameObjectStrikeThrough()))
                .ToList();

            return new Path(rangeAttackPath);
        }

        public InventoryItem GetEquippedItem()
        {
            var equippedItem = Inventory.EquippedWeapon;
            
            if (equippedItem == null)
                return null;

            return Inventory.GetInventoryItems().First(i => i.ItemId == Inventory.EquippedWeapon.ID);
        }

        protected IEnumerable<CommandResult> NextTurn()
        {
            LastMonstersInView = MonstersInView;

            foreach (var monster in Monsters.LiveMonsters.Where(m => m.CurrentMap.Equals(CurrentMap)))
            {
                if (Player.IsDead)
                    yield break;

                foreach (var command in monster.NextTurn(CommandCollection))
                {
                    foreach (var result in ExecuteCommand(command))
                        yield return result;
                }
            }

            foreach (var environmentalEffect in EnvironmentalEffects.Values.Where(m => m.CurrentMap.Equals(CurrentMap)).ToList())
            {
                foreach (var command in environmentalEffect.NextTurn(CommandCollection))
                {
                    foreach (var result in ExecuteCommand(command))
                        yield return result;
                }
                
                if (environmentalEffect.IsRemoved)
                    EnvironmentalEffects.Remove(environmentalEffect.ID);
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

            MessageLog.AddMessages(rechargedItems);
        }

        protected void UpdateMonstersInView()
        {
            MonstersInView = Monsters.LiveMonsters
                .Where(m => m.CurrentMap.Equals(CurrentMap))
                .Where(m => CurrentMap.PlayerFOV.BooleanResultView[m.Position])
                .ToList();
        }

        private IEnumerable<CommandResult> ExecuteCommand(BaseGameActionCommand command)
        {
            var result = command.Execute();
            MessageLog.AddMessages(result.Messages);
            RadioComms.ProcessCommand(command);

            yield return result;

            foreach (var subsequentCommand in result.SubsequentCommands)
            {
                foreach (var subsequentResult in ExecuteCommand(subsequentCommand))
                    yield return subsequentResult;
            }

            if (command.EndsPlayerTurn && result.Result == CommandResultEnum.Success)
            {
                UpdateFieldOfView();

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

        public IList<RadioCommsItem> GetNewRadioCommsItems()
        {
            return RadioComms
                .GetNewRadioComms()
                .Select(s => new RadioCommsItem(s))
                .ToList();;
        }

        public IGridView<double?> GetGoalMap()
        {
            return Monsters.First().Value.GetGoalMap();
        }

        public IList<CommandResult> ForceLevelChange(ForceLevelChange forceLevelChange)
        {
            var mapExit = MapExits.ForMap(CurrentMap)
                .Where(e =>
                    forceLevelChange == Components.ForceLevelChange.NextLevel
                        ? e.Destination.CurrentMap.MarsMap().Level > CurrentMap.Level 
                        : e.Destination.CurrentMap.MarsMap().Level < CurrentMap.Level)
                .OrderBy(exit => exit.ID)
                .FirstOrDefault();

            if (mapExit == null)
                return new List<CommandResult>();
            
            var command = CommandCollection.CreateCommand<ChangeMapCommand>(this);
            command.Initialise(Player, mapExit);
            
            return ExecuteCommand(command).ToList();
        }

        public void CreateWall(Point position)
        {
            CurrentMap.CreateWall(WallType.RockWall, position, GameObjectFactory);
            UpdateFieldOfView();
        }

        public void CreateFloor(Point position)
        {
            CurrentMap.CreateFloor(FloorType.BlankFloor, position, GameObjectFactory);
            UpdateFieldOfView();
        }

        public void CreateWall(int x, int y)
        {
            CurrentMap.CreateWall(WallType.RockWall, new Point(x, y), GameObjectFactory);
            UpdateFieldOfView();
        }

        public void CreateFloor(int x, int y)
        {
            CurrentMap.CreateFloor(FloorType.BlankFloor, new Point(x, y), GameObjectFactory);
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

                // Load the command collection, store the replay commands from it then
                // re-initialise it to clear it out for the replay execution
                CommandCollection.LoadState(SaveGameService, this);
                
                CommandCollection.Initialise();
            }

            return loadGameResult;
        }

        public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            Reset();

            GameObjectFactory.LoadState(saveGameService, gameWorld);
            Walls.LoadState(saveGameService, gameWorld);
            Floors.LoadState(saveGameService, gameWorld);
            Doors.LoadState(saveGameService, gameWorld);
            Features.LoadState(saveGameService, gameWorld);
            Monsters.LoadState(saveGameService, gameWorld);
            Items.LoadState(saveGameService, gameWorld);
            Machines.LoadState(saveGameService, gameWorld);
            EnvironmentalEffects.LoadState(saveGameService, gameWorld);
            MapExits.LoadState(saveGameService, gameWorld);
            Ships.LoadState(saveGameService, gameWorld);

            var playerSaveData = saveGameService.GetFromStore<PlayerSaveData>();

            // Inventory must be loaded before player as player recalculates attacks based on inventory
            Inventory = new Inventory(this);
            Inventory.LoadState(saveGameService, gameWorld);

            Player = GameObjectFactory.CreateGameObject<Player>(playerSaveData.State.Id);
            Player.LoadState(saveGameService, gameWorld);

            MessageLog.LoadState(saveGameService, gameWorld);
            RadioComms.LoadState(saveGameService, gameWorld);

            Maps.LoadState(saveGameService, gameWorld);
            GameTimeService.LoadState(saveGameService);
            CommandCollection.LoadState(saveGameService, gameWorld);

            Story.LoadState(saveGameService, gameWorld);
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
            Doors.SaveState(saveGameService, gameWorld);
            Features.SaveState(saveGameService, gameWorld);
            Monsters.SaveState(saveGameService, gameWorld);
            Items.SaveState(saveGameService, gameWorld);
            Machines.SaveState(saveGameService, gameWorld);
            EnvironmentalEffects.SaveState(saveGameService, gameWorld);
            MapExits.SaveState(saveGameService, gameWorld);
            Ships.SaveState(saveGameService, gameWorld);
            MessageLog.SaveState(saveGameService, gameWorld);
            RadioComms.SaveState(saveGameService, gameWorld);
            Player.SaveState(saveGameService, gameWorld);
            CommandCollection.SaveState(saveGameService, gameWorld);
            Inventory.SaveState(saveGameService, gameWorld);
            Maps.SaveState(saveGameService, gameWorld);
            GameTimeService.SaveState(saveGameService);
            Story.SaveState(saveGameService, gameWorld);
            
            var gameWorldSaveData = GetSaveState();
            saveGameService.SaveToStore(gameWorldSaveData);

            var headerSaveDataMemento = new Memento<HeaderSaveData>(new HeaderSaveData());
            headerSaveDataMemento.State.LoadGameDetail = LoadGameDetail;
            saveGameService.SaveHeaderToStore(headerSaveDataMemento);
        }

        public IMemento<GameWorldSaveData> GetSaveState()
        {
            var memento = new Memento<GameWorldSaveData>(new GameWorldSaveData());
            memento.State.GameId = GameId;
            memento.State.Seed = Seed;
            memento.State.RandomNumberGenerator = new MizuchiRandom(((MizuchiRandom)GlobalRandom.DefaultRNG).StateA, ((MizuchiRandom)GlobalRandom.DefaultRNG).StateB); ;
            memento.State.MonstersInView = MonstersInView.Select(m => m.ID).ToList();
            memento.State.LastMonstersInView = LastMonstersInView.Select(m => m.ID).ToList();
            return memento;
        }

        public void SetLoadState(IMemento<GameWorldSaveData> memento)
        {
            GameId = memento.State.GameId;
            Seed = memento.State.Seed;
            LastMonstersInView = memento.State.LastMonstersInView.Select(m => Monsters[m]).ToList();
            MonstersInView = memento.State.MonstersInView.Select(m => Monsters[m]).ToList();
            GlobalRandom.DefaultRNG = new MizuchiRandom(memento.State.RandomNumberGenerator.StateA, memento.State.RandomNumberGenerator.StateB);
        }

        public PlayerStatus GetPlayerStatus()
        {
            var playerStatus = new PlayerStatus
            {
                Health = Player.Health,
                IsDead = Player.IsDead,
                IsVictorious = Player.IsVictorious,
                MaxHealth = Player.MaxHealth,
                Shield = Player.Shield,
                Name = Player.Name,
                Position = Player.Position,
                AmbientText = string.Empty
            };

            var feature = CurrentMap.GetObjectAt<Feature>(Player.Position);

            if (feature != null)
                playerStatus.AmbientText = feature.GetAmbientText();

            return playerStatus;
        }

        public IList<MonsterStatus> GetStatusOfMonstersInView()
        {
            var status = MonstersInView
                .Select(
                    m =>
                    {
                        var monsterStatus = m.GetMonsterStatus(Player);

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

        public string GetGameObjectTooltipAt(Point point)
        {
            if (!CurrentMap.Bounds().Contains(point))
                return null;
            
            var text = new StringBuilder();

            var gameObjects = CurrentMap.GetObjectsAt(point);

            if (gameObjects.FirstOrDefault(g => g is Machine) is Machine machine)
            {
                text.AppendLine(machine.MachineType.GetLongDescription());
                return text.ToString();
            }

            if (gameObjects.FirstOrDefault(g => g is Machine) is MapExit mapExit)
            {
                text.AppendLine($"I see an exit going {mapExit.MapExitType.DirectionText.ToLower()}");

                return text.ToString();
            }
            
            if (gameObjects.FirstOrDefault(g => g is Monster) is Monster monster)
            {
                text.AppendLine(monster.GetInformation(Player));
            }
            
            if (gameObjects.FirstOrDefault(g => g is Item) is Item item)
            {
                text.AppendLine(Inventory.GetItemDescription(item, 1));
            }
            
            if (gameObjects.FirstOrDefault(g => g is Feature) is Feature feature)
            {
                text.AppendLine(feature.GetAmbientText());
            }
            
            return text.ToString();
        }

        public List<InventoryItem> GetInventoryItems()
        {
            return Inventory.GetInventoryItems();
        }

        public IList<CommandResult> DropItemRequest(Keys itemKey)
        {
            if (!Inventory.ItemKeyAssignments.TryGetValue(itemKey, out var itemGroup))
                return null;

            var dropItemCommand = CommandCollection.CreateCommand<DropItemCommand>(this);

            dropItemCommand.Initialise(Player, itemGroup.First());

            return ExecuteCommand(dropItemCommand).ToList();
        }

        public IList<CommandResult> EquipItemRequest(Keys itemKey)
        {
            if (!Inventory.ItemKeyAssignments.TryGetValue(itemKey, out var itemGroup))
                return null;

            var equipItemCommand = CommandCollection.CreateCommand<EquipItemCommand>(this);

            equipItemCommand.Initialise(itemGroup.First());

            return ExecuteCommand(equipItemCommand).ToList();
        }

        public IList<CommandResult> UnequipItemRequest(Keys itemKey)
        {
            if (!Inventory.ItemKeyAssignments.TryGetValue(itemKey, out var itemGroup))
                return null;

            var unequipItemCommand = CommandCollection.CreateCommand<UnequipItemCommand>(this);

            unequipItemCommand.Initialise(itemGroup.First());

            return ExecuteCommand(unequipItemCommand).ToList();
        }

        public IList<CommandResult> ApplyItemRequest(Keys itemKey)
        {
            Item item;
            
            if (Inventory.ItemKeyAssignments.TryGetValue(itemKey, out var itemGroup))
            {
                item = itemGroup.First();
            }
            else
            {
                Inventory.HotBarKeyAssignments.TryGetValue(itemKey, out item);
            }

            var applyItemCommand = CommandCollection.CreateCommand<ApplyItemCommand>(this);

            applyItemCommand.Initialise(Player, item);

            return ExecuteCommand(applyItemCommand).ToList();
        }

        /// <summary>
        /// ApplyItemRequest is done first. If is an enchant potion and is successful then
        /// it is sent back in the command result. The UI will then ask for an item to enchant.
        /// This choice is then sent here.
        /// </summary>
        public IList<CommandResult> EnchantItemRequest(Keys itemKey)
        {
            if (!Inventory.ItemKeyAssignments.TryGetValue(itemKey, out var item))
                return null;
            
            var enchantItemCommand = CommandCollection.CreateCommand<EnchantItemCommand>(this);

            enchantItemCommand.Initialise(item.First());

            return ExecuteCommand(enchantItemCommand).ToList();
        }

        public void Regenerate()
        {
            Player.Regenerate();
            
            foreach (var monster in Monsters.Values.Where(m => !m.IsDead))
            {
                monster.Regenerate();
            }
        }

        public void SpawnMonster(SpawnMonsterParams spawnMonsterParams)
        {
            if (spawnMonsterParams.MapId == Guid.Empty)
                spawnMonsterParams.MapId = CurrentMap.Id;
            
            Spawner.SpawnMonster(spawnMonsterParams);
        }

        public void SpawnItem(SpawnItemParams spawnItemParams)
        {
            if (spawnItemParams.MapId == Guid.Empty)
                spawnItemParams.MapId = CurrentMap.Id;
            
            Spawner.SpawnItem(spawnItemParams);
        }

        public void SpawnMachine(SpawnMachineParams spawnMachineParams)
        {
            if (spawnMachineParams.MapId == Guid.Empty)
                spawnMachineParams.MapId = CurrentMap.Id;
            
            Spawner.SpawnMachine(spawnMachineParams);
        }
        
        public void SpawnEnvironmentalEffect(SpawnEnvironmentalEffectParams spawnEnvironmentalEffectParams)
        {
            if (spawnEnvironmentalEffectParams.MapId == Guid.Empty)
                spawnEnvironmentalEffectParams.MapId = CurrentMap.Id;
            
            Spawner.SpawnEnvironmentalEffect(spawnEnvironmentalEffectParams);
        }
        
        public IList<CommandResult> IdentifyItemRequest(Keys requestKey)
        {
            if (!Inventory.ItemKeyAssignments.TryGetValue(requestKey, out var item))
                return null;
            
            var identifyItemCommand = CommandCollection.CreateCommand<IdentifyItemCommand>(this);

            identifyItemCommand.Initialise(item.First());

            return ExecuteCommand(identifyItemCommand).ToList();
        }

        public void SpawnMapExit(SpawnMapExitParams spawnMapExitParams)
        {
            if (spawnMapExitParams.MapId == Guid.Empty)
                spawnMapExitParams.MapId = CurrentMap.Id;
            
            Spawner.SpawnMapExit(spawnMapExitParams);
        }

        public void CancelIdentify()
        {
            // Restore the machine's used item status to unused
            var lastCommand = CommandCollection.GetLastCommand<ApplyMachineCommand>();
            
            var undoCommand = CommandCollection.CreateCommand<UndoCommand>(this);
            undoCommand.Initialise(lastCommand.Id);
            ExecuteCommand(undoCommand).ToList();
        }

        public Rectangle GetCurrentMapDimensions()
        {
            return Rectangle.WithPositionAndSize(new Point(0, 0), CurrentMap.Width, CurrentMap.Height);
        }
    }
}
