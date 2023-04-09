using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Game.Commands
{
    public class ApplyItemCommandSaveData : BaseCommandSaveData
    {
        public uint ItemId { get; set; }
        public uint GameObjectId { get; set; }
        public bool CanApply { get; set; }
        public bool IsItemTypeDiscovered { get; set; }
        public Keys ItemKey { get; set; }
        public bool IsCharged { get; set; }
    }
}
