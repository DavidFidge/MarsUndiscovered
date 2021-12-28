using System;
using System.Collections.Generic;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Components;

using SadRogue.Primitives;

namespace MarsUndiscovered.Interfaces
{
    public interface IGameWorld : ILoadGameDetail, IBaseComponent
    {
        Player Player { get; set; }
        void NewGame(uint? seed = null);
        Map Map { get; }
        void MoveRequest(Direction direction);
        IGameTurnService GameTurnService { get; }
        GameObjectCollection GameObjects { get; }
        IList<string> GetMessagesSince(int currentCount);
        SaveGameResult SaveGame(string saveGameName, bool overwrite);
        LoadGameResult LoadGame(string saveGameName);
        uint Seed { get; }
    }
}