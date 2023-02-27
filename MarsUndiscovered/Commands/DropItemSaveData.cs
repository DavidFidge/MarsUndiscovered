using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Commands
{
    public class DropItemSaveData : BaseCommandSaveData
    {
        public uint ItemId { get; set; }
        public uint GameObjectId { get; set; }
        public bool WasInInventory { get; set; }
        public bool WasEquipped { get; set; }
        public bool OldHasBeenDropped { get; set; }
        public Keys ItemKey { get; set; }
    }
}
