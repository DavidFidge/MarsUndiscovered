using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class LightningAttackCommandSaveData : BaseCommandSaveData
    {
        public uint SourceId { get; set; }
        public List<Point> Path { get; set; }
        public List<AttackData> LightningAttackData { get; set; } = new List<AttackData>();
    }
}
