namespace MarsUndiscovered.Game.Commands
{
    public class EnchantItemCommandSaveData : BaseCommandSaveData
    {
        // Item that will be enchanted
        public uint TargetId { get; set; }
        
        public int OldEnchantLevel { get; set; }
        public int NewEnchantLevel { get; set; }
    }
}
