namespace MarsUndiscovered.Commands
{
    public class LightningAttackCommandSaveData : BaseCommandSaveData
    {
        public uint SourceId { get; set; }
        public uint TargetId { get; set; }
    }
}
