using SadRogue.Primitives;

namespace MarsUndiscovered.Components.SaveData
{
    public class GameObjectSaveData
    {
        // We only need to save position. Do not need CurrentMap.Id. The map save data will
        // save the id's of all game objects in the map.
        public Point Position { get; set; }
        public bool IsWalkable { get; set; }
        public bool IsTransparent { get; set; }
        public uint Id { get; set; }
    }
}
