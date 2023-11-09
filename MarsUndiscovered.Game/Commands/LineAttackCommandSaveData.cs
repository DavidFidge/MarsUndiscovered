using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class LineAttackCommandSaveData : BaseCommandSaveData
    {
        public uint SourceId { get; set; }
        public List<Point> Path { get; set; }
        public List<AttackRestoreData> LineAttackCommandRestore { get; set; } = new List<AttackRestoreData>();
    }
}
