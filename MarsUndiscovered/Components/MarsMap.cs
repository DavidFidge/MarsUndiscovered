using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Components.Factories;
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

        public IList<Wall> Walls { get; private set; }
        public IList<Floor> Floors { get; private set; }

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

            Walls = walls.Where(w => !w.IsDestroyed).ToList();
            Floors = floors.Where(w => !w.IsDestroyed).ToList();

            var wallsFloors = Walls.Cast<Terrain>()
                .Union(Floors)
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

        public Wall CreateWall(Point position, IGameObjectFactory gameObjectFactory)
        {
            var wall = GetTerrainAt<Wall>(position);

            if (wall != null)
                return wall;

            wall = gameObjectFactory.CreateWall();
            wall.Position = position;
            wall.Index = position.ToIndex(Width);
            Walls.Add(wall);
            _gameWorld.Walls.Add(wall.ID, wall);

            DestroyFloor(position);
            SetTerrain(wall);
            return wall;
        }

        public Floor CreateFloor(Point position, IGameObjectFactory gameObjectFactory)
        {
            var floor = GetTerrainAt<Floor>(position);

            if (floor != null)
                return floor;

            floor = gameObjectFactory.CreateFloor();
            floor.Position = position;
            floor.Index = position.ToIndex(Width);
            Floors.Add(floor);
            _gameWorld.Floors.Add(floor.ID, floor);

            DestroyWall(position);
            SetTerrain(floor);
            return floor;
        }

        private void DestroyWall(Point position)
        {
            var wall = GetObjectAt<Wall>(position);

            if (wall == null)
                return;

            wall.IsDestroyed = true;
            RemoveTerrain(wall);
        }

        private void DestroyFloor(Point position)
        {
            var floor = GetObjectAt<Floor>(position);

            if (floor == null)
                return;

            floor.IsDestroyed = true;
            RemoveTerrain(floor);
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

        public IList<T> GetTerrainAround<T>(Point position, AdjacencyRule adjacencyRule) where T : Terrain
        {
            return adjacencyRule.Neighbors(position)
                .Where(p => this.Bounds().Contains(p))
                .Select(GetTerrainAt<T>)
                .Where(s => s != null)
                .ToList();
        }

        public Point FindClosestFreeFloor(Point startPoint, bool contiguousFromStartingPoint = true)
        {
            var queue = new Queue<Point>();
            var explored = new HashSet<Point>();
            var wallPoints = new Queue<Point>();

            queue.Enqueue(startPoint);
            explored.Add(startPoint);

            var adjacencyRule = AdjacencyRule.EightWay;
            var enteredLoop = false;

            while (wallPoints.Count > 0 || !enteredLoop)
            {
                enteredLoop = true;

                while (queue.Count > 0)
                {
                    var point = queue.Dequeue();

                    if (WalkabilityView[point])
                        return point;

                    var neighbours = adjacencyRule.Neighbors(point);

                    foreach (var neighbor in neighbours)
                    {
                        if (this.Bounds().Contains(neighbor) && !explored.Contains(neighbor))
                        {
                            if (GetTerrainAt(neighbor) is Floor)
                            {
                                queue.Enqueue(neighbor);
                            }
                            else
                            {
                                wallPoints.Enqueue(neighbor);
                            }

                            explored.Add(neighbor);
                        }
                    }
                }

                if (!contiguousFromStartingPoint)
                {
                    while (wallPoints.Count > 0)
                        queue.Enqueue(wallPoints.Dequeue());
                }
            }

            return Point.None;
        }
    }
}
