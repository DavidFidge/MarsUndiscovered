using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Extensions;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.Components
{
    public class MarsMap : Map, IEquatable<MarsMap>, IMementoState<MapSaveData>
    {
        public static readonly int MapWidth = 84;
        public static readonly int MapHeight = 26;
        private readonly IGameWorld _gameWorld;

        public Guid Id { get; set; }
        public int Level { get; set; }
        public ArrayView<SeenTile> SeenTiles { get; set; }

        public MarsMap(IGameWorld gameWorld) : base(MapWidth, MapHeight, 3, Distance.Chebyshev, UInt32.MaxValue, UInt32.MaxValue, 0)
        {
            Id = Guid.NewGuid();
            Level = 1;
            _gameWorld = gameWorld;

            var seenTiles = this.Positions()
                .Select(p => new SeenTile(p))
                .ToArray();

            SeenTiles = new ArrayView<SeenTile>(seenTiles, MapWidth);
        }

        public void ApplyTerrainOverlay(IEnumerable<Wall> walls, IEnumerable<Floor> floors)
        {
            Debug.Assert(floors.Any() || walls.Any(), "Walls and/or Floors must be populated");

            var wallsFloors = walls.Cast<Terrain>()
                .Union(floors)
                .Where(t => !t.IsDestroyed)
                .OrderBy(t => t.Index)
                .ToArrayView(Width);

            ApplyTerrainOverlay(wallsFloors);
        }

        public IList<IGameObject> LastSeenGameObjectsAtPosition(Point position)
        {
            return SeenTiles[position].LastSeenGameObjects;
        }

        public bool Equals(MarsMap other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MarsMap)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public void UpdateFieldOfView(Point position)
        {
            PlayerFOV.Calculate(position);

            UpdateSeenTiles(PlayerFOV.CurrentFOV);
        }

        public void UpdateSeenTiles(IEnumerable<Point> visiblePoints)
        {
            foreach (var point in visiblePoints)
            {
                var seenTile = SeenTiles[point];

                seenTile.HasBeenSeen = true;
                seenTile.LastSeenGameObjects = GetObjectsAt(point).ToList();
            }
        }

        public IMemento<MapSaveData> GetSaveState(IMapper mapper)
        {
            var memento = Memento<MapSaveData>.CreateWithAutoMapper(this, mapper);

            return new Memento<MapSaveData>(memento.State);
        }

        public void SetLoadState(IMemento<MapSaveData> memento, IMapper mapper)
        {
            Memento<MapSaveData>.SetWithAutoMapper(this, memento, mapper);

            for (var i = 0; i < memento.State.SeenTiles.Length; i++)
            {
                SeenTiles[i] = mapper.Map<SeenTile>(memento.State.SeenTiles[i]);
                SeenTiles[i].LastSeenGameObjects = memento.State.SeenTiles[i].LastSeenGameObjectIds
                    .Select(id => _gameWorld.GameObjects[id])
                    .ToList();
            }

            var gameObjectsOnMap = memento.State.GameObjectIds
                .Select(g => _gameWorld.GameObjects[g])
                .ToList();

            var terrain = gameObjectsOnMap.OfType<Terrain>().ToList();

            ApplyTerrainOverlay(terrain.OfType<Wall>(), terrain.OfType<Floor>());

            var nonTerrainObjects = gameObjectsOnMap.Except(terrain);

            foreach (var nonTerrainObject in nonTerrainObjects)
            {
                AddEntity(nonTerrainObject);
            }
        }

        public IList<uint> GetGameObjectIds()
        {
            return _gameWorld.GameObjects.Values
                .Where(g => g.CurrentMap != null)
                .Where(g => g.CurrentMap.Equals(this))
                .Select(s => s.ID)
                .ToList();
        }
    }
}
