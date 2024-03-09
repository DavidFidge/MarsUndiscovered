using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Game.Components
{
    public class InventoryItem
    {
        public Keys Key { get; set; }
        public Keys HotBarKey { get; set; }
        public string KeyDescription => $"{Key.ToString().ToLower()})";
        public string HotBarKeyDescription => $"{HotBarKey.ToString().ToLower()})";
        public string ItemDescription { get; set; }
        public string ItemDiscoveredDescription { get; set; }
        public string LongDescription { get; set; }
        public ItemType ItemType { get; set; }
        public bool CanEquip { get; set; }
        public bool CanUnequip { get; set; }
        public bool CanDrop { get; set; }
        public bool CanApply { get; set; }
        public bool CanEnchant { get; set; }
        public bool CanRangeAttack { get; set; }
        public bool CanAssignHotKey { get; set; }
    }
}