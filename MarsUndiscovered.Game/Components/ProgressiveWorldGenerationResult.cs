namespace MarsUndiscovered.Game.Components
{
    public class ProgressiveWorldGenerationResult
    {
        public ulong Seed { get; set; }
        public bool IsFinalStep { get; set; }
        public bool Failed { get; set; }
    }
}