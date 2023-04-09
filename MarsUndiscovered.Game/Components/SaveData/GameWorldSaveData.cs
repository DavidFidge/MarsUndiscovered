using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components.SaveData
{
    public class GameWorldSaveData
    {
        public Guid GameId { get; set; }
        public ulong Seed { get; set; }
        public List<uint> MonstersInView { get; set; }
        public List<uint> LastMonstersInView { get; set; }
        public MizuchiRandom RandomNumberGenerator { get; set; }
    }
}
