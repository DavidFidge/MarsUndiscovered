using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.GameFramework;
using GoRogue.Pathing;
using MarsUndiscovered.Game.Components.Dto;
using MarsUndiscovered.Interfaces;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using Point = SadRogue.Primitives.Point;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace MarsUndiscovered.Game.Components
{
    public class GameWorldEndpoint : IGameWorldEndpoint, IGameWorldConsoleCommandEndpoint
    {
        private readonly IFactory<IGameWorld> _gameWorldFactory;

        public IGameWorld GameWorld { get; private set; }

        public GameWorldEndpoint(IFactory<IGameWorld> gameWorldFactory)
        {
            _gameWorldFactory = gameWorldFactory;
        }

        public SaveGameResult SaveGame(string saveGameName, bool overwrite)
        {
            return GameWorld.SaveGame(saveGameName, overwrite);
        }

        public void LoadGame(string gameName)
        {
            GameWorld = _gameWorldFactory.Create();
            GameWorld.LoadGame(gameName);
        }

        public void NewGame(ulong? seed = null)
        {
            GameWorld = _gameWorldFactory.Create();
            GameWorld.NewGame(seed);
        }

        public ProgressiveWorldGenerationResult ProgressiveWorldGeneration(ulong? seed, int step, WorldGenerationTypeParams worldGenerationTypeParams)
        {
            GameWorld = _gameWorldFactory.Create();
            return GameWorld.ProgressiveWorldGeneration(seed, step, worldGenerationTypeParams);
        }

        public void AfterCreateGame()
        {
            GameWorld.AfterCreateGame();
        }

        public List<InventoryItem> GetInventoryItems()
        {
            return GameWorld.GetInventoryItems();
        }

        public IList<CommandResult> EquipItemRequest(Keys requestKey)
        {
            return GameWorld.EquipItemRequest(requestKey);
        }

        public IList<CommandResult> UnequipItemRequest(Keys requestKey)
        {
            return GameWorld.UnequipItemRequest(requestKey);
        }

        public IList<CommandResult> DropItemRequest(Keys requestKey)
        {
            return GameWorld.DropItemRequest(requestKey);
        }

        public IList<CommandResult> ApplyItemRequest(Keys requestKey)
        {
            return GameWorld.ApplyItemRequest(requestKey);
        }

        public string GetSeed()
        {
            return GameWorld.Seed.ToString();
        }

        public Rectangle GetCurrentMapDimensions()
        {
            return Rectangle.WithPositionAndSize(new Point(0, 0), GameWorld.CurrentMap.Width, GameWorld.CurrentMap.Height);
        }

        public IList<IGameObject> GetLastSeenGameObjectsAtPosition(Point point)
        {
            return GameWorld.GetLastSeenGameObjectsAtPosition(point);
        }

        public IList<IGameObject> GetObjectsAt(Point point)
        {
            return GameWorld.GetObjectsAt(point);
        }

        public void UpdateFieldOfView(bool partialUpdate)
        {
            GameWorld.UpdateFieldOfView(partialUpdate);
        }

        public Path GetPathToPlayer(Point mapPosition)
        {
            return GameWorld.GetPathToPlayer(mapPosition);
        }

        public IList<MonsterStatus> GetStatusOfMonstersInView()
        {
            return GameWorld.GetStatusOfMonstersInView();
        }

        public PlayerStatus GetPlayerStatus()
        {
            return GameWorld.GetPlayerStatus();
        }

        public IList<string> GetMessagesSince(int messageLogCount)
        {
            return GameWorld.GetMessagesSince(messageLogCount);
        }

        public string GetGameObjectTooltipAt(Point point)
        {
            return GameWorld.GetGameObjectTooltipAt(point);
        }

        public IList<CommandResult> MoveRequest(Direction direction)
        {
            return GameWorld.MoveRequest(direction);
        }

        public IList<CommandResult> MoveRequest(Path path)
        {
            return GameWorld.MoveRequest(path);
        }

        public AutoExploreResult AutoExploreRequest()
        {
            return GameWorld.AutoExploreRequest();
        }

        public Point GetPlayerPosition()
        {
            return GameWorld.GetPlayerPosition();
        }

        public void AfterProgressiveWorldGeneration()
        {
            GameWorld.AfterProgressiveWorldGeneration();
        }

        public async Task SendPendingMorgues()
        {
            await GameWorld.SendPendingMorgues();
        }

        public void SnapshotMorgue(string username, bool uploadMorgueFiles)
        {
            GameWorld.SnapshotMorgue(username, uploadMorgueFiles);
        }

        public Guid GetGameId()
        {
            return GameWorld.GameId;
        }

        public IList<RadioCommsItem> GetNewRadioCommsItems()
        {
            return GameWorld.GetNewRadioCommsItems();
        }

        public IGridView<double?> GetGoalMap()
        {
            return GameWorld.GetGoalMap();
        }

        public IList<CommandResult> ForceLevelChange(ForceLevelChange forceLevelChange)
        {
            return GameWorld.ForceLevelChange(forceLevelChange);
        }

        public IList<CommandResult> EnchantItemRequest(Keys requestKey)
        {
            return GameWorld.EnchantItemRequest(requestKey);
        }

        public IList<CommandResult> IdentifyItemRequest(Keys requestKey)
        {
            return GameWorld.IdentifyItemRequest(requestKey);
        }

        public void CancelIdentify()
        {
            GameWorld.CancelIdentify();
        }

        public void AssignHotBarItem(Keys inventoryItemKey, Keys requestKey)
        {
            GameWorld.AssignHotBarItem(inventoryItemKey, requestKey);
        }

        public List<InventoryItem> GetHotBarItems()
        {
            return GameWorld.GetHotBarItems();
        }

        public void RemoveHotBarItem(Keys requestKey)
        {
            GameWorld.RemoveHotBarItem(requestKey);
        }

        public IList<CommandResult> DoRangedAttack(Keys requestKey, Point target)
        {
            return GameWorld.DoRangedAttack(requestKey, target);
        }

        public Path GetPathForRangedAttack(Point mapPosition)
        {
            return GameWorld.GetPathForRangedAttack(mapPosition);
        }

        public InventoryItem GetEquippedItem()
        {
            return GameWorld.GetEquippedItem();
        }

        public void SpawnItem(SpawnItemParams spawnItemParams)
        {
            GameWorld.SpawnItem(spawnItemParams);
        }

        public void SpawnMonster(SpawnMonsterParams spawnMonsterParams)
        {
            GameWorld.SpawnMonster(spawnMonsterParams);
        }

        public void SpawnMapExit(SpawnMapExitParams spawnMapExitParams)
        {
            GameWorld.SpawnMapExit(spawnMapExitParams);
        }

        public void SpawnMachine(SpawnMachineParams spawnMachineParams)
        {
            GameWorld.SpawnMachine(spawnMachineParams);
        }
    }
}
