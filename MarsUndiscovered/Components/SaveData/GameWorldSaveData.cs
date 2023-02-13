using FrigidRogue.MonoGame.Core.Interfaces.Components;

using ShaiRandom.Generators;

namespace MarsUndiscovered.Components.SaveData
{
    public class GameWorldSaveData : ILoadGameDetail
    {
        public Guid GameId { get; set; }
        public ulong Seed { get; set; }
        public string LoadGameDetail { get; set; }
        public List<uint> MonstersInView { get; set; }
        public List<uint> LastMonstersInView { get; set; }
        public MizuchiRandom RandomNumberGenerator { get; set; }
    }
}
