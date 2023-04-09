namespace MarsUndiscovered.Game.Commands
{
    public class ApplyHealingBotsCommandSaveData : BaseCommandSaveData
    {
        public uint SourceId { get; set; }
        public uint TargetId { get; set; }
        public int OldHealth { get; set; }
        public int OldMaxHealth { get; set; }
    }
}
