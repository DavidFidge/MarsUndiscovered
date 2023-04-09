using System.Diagnostics;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Extensions;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.Game.Components
{
    public class MarsMap : Map, IEquatable<MarsMap>, IMementoState<MapSaveData>
    {
        private readonly IGameWorld _gameWorld;
        private readonly int _mapWidth;
        private readonly int _mapHeight;

        public Guid Id { get; set; }
        public int Level { get; set; }
        public ArrayView<SeenTile> SeenTiles { get; set; }

        public IList<Wall> Walls { get; private set; }
        public IList<Floor> Floors { get; private set; }

        public int MapWidth => _mapWidth;

        public int MapHeight => _mapHeight;

        public MarsMap(IGameWorld gameWorld, int mapWidth, int mapHeight)
            : base(mapWidth, mapHeight, 3, Distance.Chebyshev, null, UInt32.MaxValue, 1, 0)
        {
            Id = Guid.NewGuid();
            Level = 1;
            _gameWorld = gameWorld;
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            SeenTiles = SeenTile.CreateArrayViewFromMap(this);
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

        public void ResetFieldOfView()
        {
            PlayerFOV.Reset();
            SeenTile.ResetSeenTiles(SeenTiles);
        }

        public void UpdateFieldOfView(Point position, uint visualRange)
        {
            PlayerFOV.Calculate(position, visualRange);

            UpdateSeenTiles(PlayerFOV.CurrentFOV);
        }

        public void UpdateSeenTiles(IEnumerable<Point> visiblePoints)
        {
            foreach (var point in visiblePoints)
            {
                var seenTile = SeenTiles[point];

                seenTile.HasBeenSeen = true;
                seenTile.LastSeenGameObjects.Clear();
                
                foreach (var gameObject in GetObjectsAt(point))
                    seenTile.LastSeenGameObjects.Add(gameObject);
            }
        }

        public Wall CreateWall(Point position, IGameObjectFactory gameObjectFactory)
        {
            var wall = GetTerrainAt<Wall>(position);

            if (wall != null)
                return wall;

            if (!DestroyFloor(position))
                return null;

            wall = gameObjectFactory.CreateWall();
            wall.Position = position;
            wall.Index = position.ToIndex(Width);
            Walls.Add(wall);
            _gameWorld.Walls.Add(wall.ID, wall);

            SetTerrain(wall);
            return wall;
        }

        public Floor CreateFloor(Point position, IGameObjectFactory gameObjectFactory)
        {
            var floor = GetTerrainAt<Floor>(position);

            if (floor != null)
                return floor;

            if (!DestroyWall(position))
                return null;

            floor = gameObjectFactory.CreateFloor();
            floor.Position = position;
            floor.Index = position.ToIndex(Width);
            Floors.Add(floor);
            _gameWorld.Floors.Add(floor.ID, floor);

            SetTerrain(floor);
            return floor;
        }

        private bool DestroyWall(Point position)
        {
            var wall = GetObjectAt<Wall>(position);

            if (wall == null || !wall.IsDestroyable)
                return false;

            wall.IsDestroyed = true;
            RemoveTerrain(wall);
            return true;
        }

        private bool DestroyFloor(Point position)
        {
            var floor = GetObjectAt<Floor>(position);

            if (floor == null || !floor.IsDestroyable)
                return false;

            floor.IsDestroyed = true;
            RemoveTerrain(floor);
            return true;
        }

        public IMemento<MapSaveData> GetSaveState()
        {
            var memento = new Memento<MapSaveData>(new MapSaveData());

            memento.State.Id = Id;
            memento.State.Level = Level;
            memento.State.SeenTiles = SeenTiles.ToArray()
                .Select(s => s.GetSaveState())
                .ToList();

            memento.State.GameObjectIds = _gameWorld.GameObjects.Values
                .Where(g => g.CurrentMap != null)
                .Where(g => g.CurrentMap.Equals(this))
                .Select(s => s.ID)
                .ToList();

            memento.State.Width = Width;
            memento.State.Height = Height;

            return new Memento<MapSaveData>(memento.State);
        }

        public void SetLoadState(IMemento<MapSaveData> memento)
        {
            Id = memento.State.Id;
            Level = memento.State.Level;

            var seenTiles = memento.State.SeenTiles
                .Select(s =>
                    {
                        var seenTiles = new SeenTile(s.State.Point);
                        seenTiles.SetLoadState(s, _gameWorld.GameObjects);
                        return seenTiles;
                    }
                )
                .ToArray();

            SeenTiles = new ArrayView<SeenTile>(seenTiles, MapWidth);

            var gameObjectsOnMap = memento.State.GameObjectIds
                .Select(g => _gameWorld.GameObjects[g])
                .OfType<MarsGameObject>()
                .ToList();

            var terrain = gameObjectsOnMap.OfType<Terrain>().ToList();

            ApplyTerrainOverlay(terrain.OfType<Wall>(), terrain.OfType<Floor>());

            var nonTerrainObjects = gameObjectsOnMap.Except(terrain);

            foreach (var nonTerrainObject in nonTerrainObjects)
            {
                AddEntity(nonTerrainObject);
            }

            foreach (var gameObject in gameObjectsOnMap)
            {
                gameObject.AfterMapLoaded();
            }
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
