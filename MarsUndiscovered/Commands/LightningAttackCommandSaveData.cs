using System.Collections.Generic;

using SadRogue.Primitives;

namespace MarsUndiscovered.Commands
{
    public class LightningAttackCommandSaveData : BaseCommandSaveData
    {
        public uint SourceId { get; set; }
        public List<uint> TargetIds { get; set; }
        public List<Point> Path { get; set; }
    }
}
