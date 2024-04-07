using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Game.Components.SaveData
{
    public class InventorySaveData : BaseSaveData
    {
        public List<uint> ItemIds = new List<uint>();
        public Dictionary<Keys, List<uint>> ItemKeyAssignments { get; set; }
        public Dictionary<Keys, uint> HotKeyAssignments { get; set; }
        public Dictionary<uint, string> CallItem { get; set; }
        public Dictionary<string, string> CallItemType { get; set; }
        public Dictionary<string, ItemTypeDiscovery> ItemTypeDiscoveries { get; set; }
        public uint? EquippedWeaponId { get; set; }
    }
}
