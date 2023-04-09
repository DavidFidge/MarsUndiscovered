using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class LightningAttackCommandSaveData : BaseCommandSaveData
    {
        public uint SourceId { get; set; }
        public List<Point> Path { get; set; }
        public List<AttackRestoreData> LineAttackCommandRestore { get; set; }
    }
}
