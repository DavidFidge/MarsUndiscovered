using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Components
{
    public class InventoryItem
    {
        public Keys Key { get; set; }
        public string KeyDescription { get; set; }
        public string ItemDescription { get; set; }
        public string LongDescription { get; set; }
        public ItemType ItemType { get; set; }
    }
}