using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.Game.Commands
{
    public class ExplodeTileCommandSaveData : BaseCommandSaveData
    {
        public Point Point { get; set; }
        public int Damage { get; set; }
        public bool DestroysWalls { get; set; }
        public uint? SourceId { get; set; }
    }
}
