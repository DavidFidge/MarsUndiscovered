using System;
using System.Collections.Generic;

using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Components;

using SadRogue.Primitives;

namespace MarsUndiscovered.Interfaces
{
    public interface IGameWorld
    {
        Player Player { get; set; }
        void Generate(uint? seed = null);
        Map Map { get; }
        void MoveRequest(Direction direction);
        IGameTurnService GameTurnService { get; }
        IDictionary<uint, IGameObject> GameObjects { get; }
        IList<string> GetMessagesSince(int currentCount);
        SaveGameResult SaveGame(string saveGameName, bool overwrite);
    }
}