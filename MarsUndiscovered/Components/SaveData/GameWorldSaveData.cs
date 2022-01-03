using FrigidRogue.MonoGame.Core.Interfaces.Components;

namespace MarsUndiscovered.Components.SaveData
{
    public class GameWorldSaveData : ILoadGameDetail
    {
        public uint Seed { get; set; }
        public string LoadGameDetail { get; set; }
    }
}
