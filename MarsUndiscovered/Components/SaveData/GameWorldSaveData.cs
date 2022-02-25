using FrigidRogue.MonoGame.Core.Interfaces.Components;

namespace MarsUndiscovered.Components.SaveData
{
    public class GameWorldSaveData : ILoadGameDetail
    {
        public ulong Seed { get; set; }
        public string LoadGameDetail { get; set; }
    }
}
