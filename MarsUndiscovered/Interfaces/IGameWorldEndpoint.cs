﻿using System.Collections.Generic;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;
using GoRogue.Pathing;

using MarsUndiscovered.Components;

using Microsoft.Xna.Framework.Input;

using SadRogue.Primitives;

namespace MarsUndiscovered.Interfaces
{
    public interface IGameWorldEndpoint
    {
        SaveGameResult SaveGame(string saveGameName, bool overwrite);
        public void LoadGame(string filename);
        public void LoadReplay(string filename);
        void NewGame(ulong? seed = null);
        void AfterCreateGame();
        List<InventoryItem> GetInventoryItems();
        void EquipItemRequest(Keys requestKey);
        void UnequipItemRequest(Keys requestKey);
        void DropItemRequest(Keys requestKey);
        string GetSeed();
        Rectangle GetCurrentMapDimensions();
        IList<IGameObject> GetLastSeenGameObjectsAtPosition(Point point);
        IList<IGameObject> GetObjectsAt(Point point);
        void UpdateFieldOfView(bool partialUpdate);
        Path GetPathToPlayer(Point mapPosition);
        IList<MonsterStatus> GetStatusOfMonstersInView();
        PlayerStatus GetPlayerStatus();
        IList<string> GetMessagesSince(int messageLogCount);
        string GetGameObjectInformationAt(Point point);
        IList<CommandResult> MoveRequest(Direction direction);
        IList<CommandResult> MoveRequest(Path path);
        AutoExploreResult AutoExploreRequest();
        Point GetPlayerPosition();
        bool ExecuteNextReplayCommand();
    }
}