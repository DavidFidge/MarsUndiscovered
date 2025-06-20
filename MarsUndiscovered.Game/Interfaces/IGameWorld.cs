using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.GameFramework;
using GoRogue.Pathing;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Dto;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Interfaces
{
    public interface IGameWorld : IBaseComponent
    {
        IMorgue Morgue { get; }
        Player Player { get; }
        void NewGame(ulong? seed = null);
        MarsMap CurrentMap { get; }
        IList<CommandResult> MoveRequest(Direction direction);
        IList<CommandResult> MoveRequest(Path path);
        IList<string> GetMessagesSince(int currentCount);
        SaveGameResult SaveGame(string saveGameName, bool overwrite);
        LoadGameResult LoadGame(string saveGameName);
        ulong Seed { get; }
        WallCollection Walls { get; }
        FloorCollection Floors { get; }
        DoorCollection Doors { get; }
        MonsterCollection Monsters { get; }
        ItemCollection Items { get; }
        MachineCollection Machines { get; }
        EnvironmentalEffectCollection EnvironmentalEffects { get; }
        Inventory Inventory { get; }
        MapExitCollection MapExits { get; }
        IDictionary<uint, IGameObject> GameObjects { get; }
        IGameObjectFactory GameObjectFactory { get; set; }
        MapCollection Maps { get; }
        ILevelGenerator LevelGenerator { get; set; }
        Guid GameId { get; }
        void SpawnMonster(SpawnMonsterParams spawnMonsterParams);
        void SpawnMapExit(SpawnMapExitParams spawnMapExitParams);
        IList<MonsterStatus> GetStatusOfMonstersInView();
        PlayerStatus GetPlayerStatus();
        Path GetPathToPlayer(Point mapPosition);
        string GetGameObjectTooltipAt(Point point);
        void SpawnItem(SpawnItemParams spawnItemParams);
        List<InventoryItem> GetInventoryItems();
        IList<CommandResult> DropItemRequest(Keys itemKey);
        IList<CommandResult> EquipItemRequest(Keys itemKey);
        IList<CommandResult> UnequipItemRequest(Keys itemKey);
        IList<CommandResult> ApplyItemRequest(Keys itemKey);

        void UpdateFieldOfView(bool partialUpdate = true);
        void AfterCreateGame();
        void ChangeMap(MarsMap map);
        AutoExploreResult AutoExploreRequest(bool fallbackToMapExit = true);
        IList<IGameObject> GetLastSeenGameObjectsAtPosition(Point point);
        IList<IGameObject> GetObjectsAt(Point point);
        Point GetPlayerPosition();
        ProgressiveWorldGenerationResult ProgressiveWorldGeneration(ulong? seed, int step, WorldGenerationTypeParams worldGenerationTypeParams);
        void AfterProgressiveWorldGeneration();
        Task SendPendingMorgues();
        void SnapshotMorgue(string username, bool uploadMorgueFiles);
        IList<RadioCommsItem> GetNewRadioCommsItems();
        IGridView<double?> GetGoalMap();
        IList<CommandResult> ForceLevelChange(ForceLevelChange forceLevelChange);
        IList<CommandResult> EnchantItemRequest(Keys requestKey);
        void SpawnMachine(SpawnMachineParams spawnMachineParams);
        IList<CommandResult> IdentifyItemRequest(Keys requestKey);
        ICommandCollection CommandCollection { get; }
        void CancelIdentify();
        void AssignHotBarItem(Keys inventoryItemKey, Keys requestKey);
        List<InventoryItem> GetHotBarItems();
        void RemoveHotBarItem(Keys requestKey);
        IList<CommandResult> DoRangedAttack(Keys requestKey, Point target);
        Path GetPathForRangedAttack(Point mapPosition);
        InventoryItem GetEquippedItem();
        MessageLog MessageLog { get; }
        RadioComms RadioComms { get; set; }
        void SpawnEnvironmentalEffect(SpawnEnvironmentalEffectParams spawnEnvironmentalEffectParams);
        Rectangle GetCurrentMapDimensions();
        void SpawnFeature(SpawnFeatureParams spawnFeatureParams);
    }
}
