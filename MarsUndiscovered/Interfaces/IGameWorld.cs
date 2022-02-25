using System.Collections.Generic;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;
using GoRogue.Pathing;

using MarsUndiscovered.Components;

using Microsoft.Xna.Framework.Input;

using SadRogue.Primitives;

namespace MarsUndiscovered.Interfaces
{
    public interface IGameWorld : ILoadGameDetail, IBaseComponent
    {
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
        MapCollection Maps { get; }
        Inventory Inventory { get; }
        IDictionary<uint, IGameObject> GameObjects { get; }
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
        void UpdateFieldOfView(bool partialUpdate = true);
        void AfterCreateGame();
        void ChangeMap(MarsMap map);
        AutoExploreResult AutoExploreRequest(bool fallbackToMapExit = false);
    }
}