using System;
using System.Collections.Generic;
using FrigidRogue.MonoGame.Core.Interfaces.Components;

namespace MarsUndiscovered.Components.SaveData
{
    public class MonsterGoalSaveData
    {
        public IList<IMemento<SeenTileSaveData>> SeenTiles { get; set; }
        public uint MonsterId { get; set; }
        public Type CurrentState { get; set; }
    }
}
