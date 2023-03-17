namespace MarsUndiscovered.Commands
{
    public class ApplyShieldCommandSaveData : BaseCommandSaveData
    {
        public uint SourceId { get; set; }
        public uint TargetId { get; set; }
        public int OldShieldAmount { get; set; }
    }
}
