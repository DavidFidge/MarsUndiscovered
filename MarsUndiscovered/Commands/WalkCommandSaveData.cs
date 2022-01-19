using System;
using SadRogue.Primitives;

namespace MarsUndiscovered.Commands
{
    public class WalkCommandSaveData : BaseCommandSaveData
    {
        public Direction Direction { get; set; }
        public uint PlayerId { get; set; }
        public Guid MapId { get; set; }
    }
}
