namespace MarsUndiscovered.Game.Commands
{
    public class UnequipItemSaveData : BaseCommandSaveData
    {
        public uint ItemId { get; set; }
        public bool IsNotEquipped { get; set; }
    }
}
