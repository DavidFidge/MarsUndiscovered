using System;
using System.Collections.Generic;

namespace MarsUndiscovered.Components.SaveData
{
    public class MonsterGoalSaveData
    {
        public SeenTileSaveData[] SeenTiles { get; set; }
        public uint MonsterId { get; set; }
        public Type CurrentState { get; set; }
    }
}
