using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class MapCollection : HashSet<MarsMap>, ISaveable
    {
        public MarsMap CurrentMap { get; set; }

        public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            var memento = GetSaveState();
            saveGameService.SaveToStore(memento);
        }

        public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            var mapCollectionSaveData = saveGameService.GetFromStore<MapCollectionSaveData>();
            SetLoadState(mapCollectionSaveData, gameWorld);
        }

        public IMemento<MapCollectionSaveData> GetSaveState()
        {
            var memento = new Memento<MapCollectionSaveData>(new MapCollectionSaveData());
            memento.State.Maps = this.Select(m => m.GetSaveState()).ToList();
            memento.State.CurrentMapId = CurrentMap.Id;

            return new Memento<MapCollectionSaveData>(memento.State);
        }

        public void SetLoadState(IMemento<MapCollectionSaveData> memento, IGameWorld gameWorld)
        {
            foreach (var mapSaveData in memento.State.Maps)
            {
                var map = new MarsMap(gameWorld, mapSaveData.State.Width, mapSaveData.State.Height);
                map.SetLoadState(mapSaveData);

                if (map.Id == memento.State.CurrentMapId)
                    CurrentMap = map;

                Add(map);
            }
        }
    }
}
