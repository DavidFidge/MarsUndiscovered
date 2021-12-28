using FrigidRogue.MonoGame.Core.Interfaces.Components;

namespace MarsUndiscovered.Components.SaveData
{
    public class GameWorldSaveData : ILoadGameDetail
    {
        public uint NextId { get; set; }
        public uint Seed { get; set; }
        public string LoadGameDetail { get; set; }
    }
}
