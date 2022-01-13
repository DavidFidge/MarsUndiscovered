using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Components.SaveData
{
    public class InventorySaveData : GameObjectSaveData
    {
        public List<uint> ItemIds = new List<uint>();
        public Dictionary<Keys, List<uint>> ItemKeyAssignments { get; set; }
        public Dictionary<uint, string> CallItem { get; set; }
        public Dictionary<string, string> CallItemType { get; set; }
    }
}
