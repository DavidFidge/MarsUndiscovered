using System.Collections.Generic;

using GoRogue.GameFramework;

using SadRogue.Primitives;

namespace MarsUndiscovered.Components.SaveData
{
    public class MapSeenTilesSaveData
    {
        public SeenTileSaveData[] SeenTiles { get; set; }
    }

    public class SeenTileSaveData
    {
        public bool HasBeenSeen { get; set; }
        public Point Point { get; set; }
        public IList<uint> LastSeenGameObjectIds { get; set; } = new List<uint>();
    }
}
