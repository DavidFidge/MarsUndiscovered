namespace MarsUndiscovered.Game.Commands
{
    public class EnchantItemCommandSaveData : BaseCommandSaveData
    {
        // The flask
        public uint SourceId { get; set; }
        
        // Item that will be enchanted
        public uint TargetId { get; set; }
        
        public int OldEnchantLevel { get; set; }
        public int NewEnchantLevel { get; set; }
    }
}
