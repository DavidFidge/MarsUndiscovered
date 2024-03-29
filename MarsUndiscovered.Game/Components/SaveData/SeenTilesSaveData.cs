using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.SaveData
{
    public class SeenTileSaveData
    {
        public bool HasBeenSeen { get; set; }
        public Point Point { get; set; }
        public IList<uint> LastSeenGameObjectIds { get; set; } = new List<uint>();
    }
}
