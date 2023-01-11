using FrigidRogue.MonoGame.Core.Interfaces.Components;

namespace MarsUndiscovered.Components.SaveData
{
    public class MapSaveData : BaseSaveData
    {
        public IList<uint> GameObjectIds { get; set; }
        public IList<IMemento<SeenTileSaveData>> SeenTiles { get; set; }
        public Guid Id { get; set; }
        public int Level { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
