using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class ChangeMapSaveData : BaseCommandSaveData
    {
        public uint GameObjectId { get; set; }
        public uint MapExitId { get; set; }
        public Guid OldMapId { get; set; }
        public Point OldPosition { get; set; }
    }
}
