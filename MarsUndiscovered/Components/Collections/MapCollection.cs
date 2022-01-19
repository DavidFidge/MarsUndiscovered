using System;
using System.Collections.Generic;

using AutoMapper;

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
            var memento = GetSaveState(saveGameService.Mapper);
            saveGameService.SaveToStore(memento);
        }

        public void LoadState(ISaveGameService saveGameService)
        {
            var mapCollectionSaveData = saveGameService.GetFromStore<MapCollectionSaveData>();
            SetLoadState(mapCollectionSaveData, saveGameService.Mapper);
        }

        public IMemento<MapCollectionSaveData> GetSaveState(IMapper mapper)
        {
            var memento = Memento<MapCollectionSaveData>.CreateWithAutoMapper(this, mapper);

            return new Memento<MapCollectionSaveData>(memento.State);
        }

        public void SetLoadState(IMemento<MapCollectionSaveData> memento, IMapper mapper)
        {
            Memento<MapCollectionSaveData>.SetWithAutoMapper(this, memento, mapper);

            foreach (var mapSaveData in memento.State.Maps)
            {
                var map = new MarsMap(_gameWorld);
                map.SetLoadState(new Memento<MapSaveData>(mapSaveData), mapper);

                if (map.Id == memento.State.CurrentMapId)
                    CurrentMap = map;

                Add(map);
            }
        }
    }
}
