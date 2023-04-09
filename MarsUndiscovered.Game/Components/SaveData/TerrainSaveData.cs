namespace MarsUndiscovered.Game.Components.SaveData
{
    public class TerrainSaveData : GameObjectSaveData
    {
        public int Index { get; set; }
        public bool IsDestroyed { get; set; }
        public bool IsDestroyable { get; set; }
    }
}
