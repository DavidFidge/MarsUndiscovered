using FrigidRogue.MonoGame.Core.Interfaces.Components;

namespace MarsUndiscovered.Game.Components.SaveData
{
    public class MapCollectionSaveData : BaseSaveData
    {
        public Guid CurrentMapId { get; set; }
        public IList<IMemento<MapSaveData>> Maps { get; set; }
    }
}
