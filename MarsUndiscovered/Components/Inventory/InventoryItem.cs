using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Components
{
    public class InventoryItem
    {
        public Keys Key { get; set; }
        public string KeyDescription => $"{Key.ToString().ToLower()})";
        public string ItemDescription { get; set; }
        public string ItemDiscoveredDescription { get; set; }
        public string LongDescription { get; set; }
        public ItemType ItemType { get; set; }
        public bool CanEquip { get; set; }
        public bool CanUnequip { get; set; }
        public bool CanDrop { get; set; }
    }
}