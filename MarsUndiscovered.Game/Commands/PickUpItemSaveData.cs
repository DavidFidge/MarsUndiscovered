namespace MarsUndiscovered.Game.Commands
{
    public class PickUpItemSaveData : BaseCommandSaveData
    {
        public uint ItemId { get; set; }
        public uint GameObjectId { get; set; }
    }
}
