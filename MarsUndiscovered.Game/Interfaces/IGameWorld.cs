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
        MonsterCollection Monsters { get; }
        ItemCollection Items { get; }
        Inventory Inventory { get; }
        IDictionary<uint, IGameObject> GameObjects { get; }
        Guid GameId { get; }
        void SpawnMonster(SpawnMonsterParams spawnMonsterParams);
        LoadGameResult LoadReplay(string saveGameName);
        bool ExecuteNextReplayCommand();
        IList<MonsterStatus> GetStatusOfMonstersInView();
        PlayerStatus GetPlayerStatus();
        Path GetPathToPlayer(Point mapPosition);
        string GetGameObjectInformationAt(Point point);
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
        Task WriteMorgueToFile(Guid gameId);
        Task SendMorgueToWeb(Guid gameId);
        void SnapshotMorgue(string username);
        IList<RadioCommsItem> GetNewRadioCommsItems();
        IGridView<double?> GetGoalMap();
    }
}