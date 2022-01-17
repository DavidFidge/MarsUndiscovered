using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components
{
    public class MapSeenTiles : ISaveable, IMementoState<MapSeenTilesSaveData>
    {
        private Map _map;
        private IGameWorld _gameWorld;
        public ArrayView<SeenTile> SeenTiles { get; set; }

        public MapSeenTiles(Map map, IGameWorld gameWorld)
        {
            _gameWorld = gameWorld;
            _map = map;
            var seenTiles = map
                .Positions()
                .Select(p => new SeenTile(p))
                .ToArray();

            SeenTiles = new ArrayView<SeenTile>(seenTiles, map.Width);
        }

        public void Update(IEnumerable<Point> visiblePoints)
        {
            foreach (var point in visiblePoints)
            {
                var seenTile = SeenTiles[point];

                seenTile.HasBeenSeen = true;
                seenTile.LastSeenGameObjects = _map.GetObjectsAt(point).ToList();
            }
        }

        public void SaveState(ISaveGameService saveGameService)
        {
            saveGameService.SaveToStore(GetSaveState(saveGameService.Mapper));
        }

        public void LoadState(ISaveGameService saveGameService)
        {
            var saveData = saveGameService.GetFromStore<MapSeenTilesSaveData>();
            SetLoadState(saveData, saveGameService.Mapper);
        }

        public IMemento<MapSeenTilesSaveData> GetSaveState(IMapper mapper)
        {
            var memento = Memento<MapSeenTilesSaveData>.CreateWithAutoMapper(this, mapper);

            return new Memento<MapSeenTilesSaveData>(memento.State);
        }

        public void SetLoadState(IMemento<MapSeenTilesSaveData> memento, IMapper mapper)
        {
            Memento<MapSeenTilesSaveData>.SetWithAutoMapper(this, memento, mapper);

            for (var i = 0; i < memento.State.SeenTiles.Length; i++)
            {
                SeenTiles[i] = mapper.Map<SeenTile>(memento.State.SeenTiles[i]);
                SeenTiles[i].LastSeenGameObjects = memento.State.SeenTiles[i].LastSeenGameObjectIds
                    .Select(id => _gameWorld.GameObjects[id])
                    .ToList();
            }
        }
    }
}