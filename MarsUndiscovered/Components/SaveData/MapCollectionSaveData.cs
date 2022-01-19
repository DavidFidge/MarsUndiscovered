using System;
using System.Collections.Generic;

namespace MarsUndiscovered.Components.SaveData
{
    public class MapCollectionSaveData : BaseSaveData
    {
        public Guid CurrentMapId { get; set; }
        public IList<MapSaveData> Maps { get; set; }
    }
}
