using System;

namespace MarsUndiscovered.Commands
{
    public class AttackCommandSaveData : BaseCommandSaveData
    {
        public uint SourceId { get; set; }
        public uint TargetId { get; set; }
    }
}
