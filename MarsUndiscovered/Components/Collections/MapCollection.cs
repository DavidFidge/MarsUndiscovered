using System.Collections.Generic;
using System.Linq;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class MapCollection : HashSet<MarsMap>, ISaveable, IMementoState<MapCollectionSaveData>
    {
        private readonly IGameWorld _gameWorld;
        public MarsMap CurrentMap { get; set; }

        public MapCollection(IGameWorld gameWorld)
        {
            _gameWorld = gameWorld;
        }

        public void SaveState(ISaveGameService saveGameService)
        {
            var memento = GetSaveState();
            saveGameService.SaveToStore(memento);
        }

        public void LoadState(ISaveGameService saveGameService)
        {
            var mapCollectionSaveData = saveGameService.GetFromStore<MapCollectionSaveData>();
            SetLoadState(mapCollectionSaveData);
        }

        public IMemento<MapCollectionSaveData> GetSaveState()
        {
            var memento = new Memento<MapCollectionSaveData>(new MapCollectionSaveData());
            memento.State.Maps = this.Select(m => m.GetSaveState()).ToList();
            memento.State.CurrentMapId = CurrentMap.Id;

            return new Memento<MapCollectionSaveData>(memento.State);
        }

        public void SetLoadState(IMemento<MapCollectionSaveData> memento)
        {
            foreach (var mapSaveData in memento.State.Maps)
            {
                var map = new MarsMap(_gameWorld);
                map.SetLoadState(mapSaveData);

                if (map.Id == memento.State.CurrentMapId)
                    CurrentMap = map;

                Add(map);
            }
        }
    }
}
