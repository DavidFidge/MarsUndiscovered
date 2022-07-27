namespace MarsUndiscovered.Commands
{
    public class MeleeAttackCommandSaveData : BaseCommandSaveData
    {
        public uint SourceId { get; set; }
        public uint TargetId { get; set; }
    }
}
