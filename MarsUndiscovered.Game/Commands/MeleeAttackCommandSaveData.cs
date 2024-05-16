namespace MarsUndiscovered.Game.Commands
{
    public class MeleeAttackCommandSaveData : BaseCommandSaveData
    {
        public uint SourceId { get; set; }
        public uint TargetId { get; set; }
        public uint? ItemId { get; set; }
        public AttackRestoreData AttackRestoreData { get; set; }
    }
}
