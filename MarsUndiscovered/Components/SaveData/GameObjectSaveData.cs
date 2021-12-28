using SadRogue.Primitives;

namespace MarsUndiscovered.Components.SaveData
{
    public class GameObjectSaveData
    {
        public Point Position { get; set; }
        public bool IsWalkable { get; set; }
        public bool IsTransparent { get; set; }
        public uint Id { get; set; }
        public int Layer { get; set; }
    }
}
