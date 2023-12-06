namespace MarsUndiscovered.Game.Commands
{
    public class EquipItemSaveData : BaseCommandSaveData
    {
        public uint ItemId { get; set; }
        public uint? PreviousItemId { get; set; }
        public bool IsAlreadyEquipped { get; set; }
        public bool CanEquipType { get; set; }
    }
}
