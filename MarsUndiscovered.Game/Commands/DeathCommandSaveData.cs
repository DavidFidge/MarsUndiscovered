using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class DeathCommandSaveData : BaseCommandSaveData
    {
        public uint SourceId { get; set; }
        public string KilledByMessage { get; set; }
        public Guid MapId { get; set; }
        public Point OldPosition { get; set; }
    }
}
