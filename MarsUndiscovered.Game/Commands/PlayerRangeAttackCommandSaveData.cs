using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class PlayerRangeAttackCommandSaveData : BaseCommandSaveData
    {
        public Point TargetPoint { get; set; }
        public uint ItemId { get; set; }
    }
}
