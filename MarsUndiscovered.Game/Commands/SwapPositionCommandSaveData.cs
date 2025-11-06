namespace MarsUndiscovered.Game.Commands
{
    public class SwapPositionCommandSaveData : BaseCommandSaveData
    {
        public uint SourceId { get; set; }
        public uint TargetId { get; set; }
    }
}
