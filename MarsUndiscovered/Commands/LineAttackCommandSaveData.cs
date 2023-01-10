using SadRogue.Primitives;

namespace MarsUndiscovered.Commands
{
    public class LineAttackCommandSaveData : BaseCommandSaveData
    {
        public uint SourceId { get; set; }
        public List<Point> Path { get; set; }
    }
}
