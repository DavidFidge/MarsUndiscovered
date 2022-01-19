using System;
using System.Collections.Generic;

namespace MarsUndiscovered.Components.SaveData
{
    public class MapSaveData : BaseSaveData
    {
        public IList<uint> GameObjectIds { get; set; }
        public SeenTileSaveData[] SeenTiles { get; set; }
        public Guid Id { get; set; }
        public int Level { get; set; }
    }
}
