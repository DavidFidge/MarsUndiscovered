using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class LaserAttackCommandSaveData : BaseCommandSaveData
    {
        public uint SourceId { get; set; }
        public List<Point> Path { get; set; }
        public List<AttackData> LaserAttackData { get; set; } = new List<AttackData>();
    }
}
