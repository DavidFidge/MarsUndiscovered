using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class ApplyForcePushCommandSaveData : BaseCommandSaveData
    {
        public uint SourceId { get; set; }
        public Point Point { get; set; }
        public List<(uint actorid, Point from, Point to)> PushedActors { get; set; }
        public int PushDistance { get; set; }
        public int Radius { get; set; }
    }
}
