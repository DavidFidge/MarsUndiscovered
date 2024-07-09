using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;
using GoRogue.Pathing;

using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Dto;
using Microsoft.Xna.Framework.Input;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Interfaces
{
    public interface IGameWorldEndpoint
    {
        SaveGameResult SaveGame(string saveGameName, bool overwrite);
        public void LoadGame(string filename);
        public void LoadReplay(string filename);
        void NewGame(ulong? seed = null);
        ProgressiveWorldGenerationResult ProgressiveWorldGeneration(ulong? seed, int step, WorldGenerationTypeParams worldGenerationTypeParams);
        void AfterCreateGame();
        List<InventoryItem> GetInventoryItems();
        IList<CommandResult> EquipItemRequest(Keys requestKey);
        IList<CommandResult> UnequipItemRequest(Keys requestKey);
        IList<CommandResult> DropItemRequest(Keys requestKey);
        IList<CommandResult> ApplyItemRequest(Keys requestKey);
        string GetSeed();
        Rectangle GetCurrentMapDimensions();
        IList<IGameObject> GetLastSeenGameObjectsAtPosition(Point point);
        IList<IGameObject> GetObjectsAt(Point point);
        void UpdateFieldOfView(bool partialUpdate);
        Path GetPathToPlayer(Point mapPosition);
        IList<MonsterStatus> GetStatusOfMonstersInView();
        PlayerStatus GetPlayerStatus();
        IList<string> GetMessagesSince(int messageLogCount);
        string GetGameObjectTooltipAt(Point point);
        IList<CommandResult> MoveRequest(Direction direction);
        IList<CommandResult> MoveRequest(Path path);
        AutoExploreResult AutoExploreRequest();
        Point GetPlayerPosition();
        ReplayCommandResult ExecuteNextReplayCommand();
        void AfterProgressiveWorldGeneration();
        Task SendPendingMorgues();
        void SnapshotMorgue(string username, bool uploadMorgueFiles);
        Guid GetGameId();
        IList<RadioCommsItem> GetNewRadioCommsItems();
        IGridView<double?> GetGoalMap();
        IList<CommandResult> ForceLevelChange(ForceLevelChange forceLevelChange);
        IList<CommandResult> EnchantItemRequest(Keys obj);
        IList<CommandResult> IdentifyItemRequest(Keys requestKey);
        void CancelIdentify();
        void AssignHotBarItem(Keys inventoryItemKey, Keys requestKey);
        List<InventoryItem> GetHotBarItems();
        void RemoveHotBarItem(Keys requestKey);
        IList<CommandResult> DoRangedAttack(Keys requestKey, Point target);
        Path GetPathForRangedAttack(Point mapPosition);
        InventoryItem GetEquippedItem();
    }
}
