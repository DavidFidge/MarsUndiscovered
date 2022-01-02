using System;
using SadRogue.Primitives;

namespace MarsUndiscovered.Commands
{
    public class WalkCommandSaveData
    {
        public Direction Direction { get; set; }
        public uint PlayerId { get; set; }
    }
}
