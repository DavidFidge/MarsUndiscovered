namespace MarsUndiscovered.Commands
{
    public class PickUpItemSaveData : BaseCommandSaveData
    {
        public uint ItemId { get; set; }
        public uint GameObjectId { get; set; }
    }
}
