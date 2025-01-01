using System.Diagnostics;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.GameFramework;
using GoRogue.MapGeneration;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Game.Extensions;
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
            : base(
                mapWidth,
                mapHeight,
                6,
                Distance.Chebyshev,
                null,
                UInt32.MaxValue,
                7, // Terrain, Feature and Door layers can block transparency,
                   // I think this has to be a bitmask, so 7 = bit 0, 1, 2 for layers 0, 1, 2 
                0)
        {
            Id = Guid.NewGuid();
            Level = 1;
            _gameWorld = gameWorld;
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            SeenTiles = SeenTile.CreateArrayViewFromMap(this);
        }
        
        public MarsMap WithTerrain(IEnumerable<Wall> walls, IEnumerable<Floor> floors)
        {
            Debug.Assert(floors != null && walls != null, "Walls and/or Floors must not be null");

            Walls = walls.Where(w => !w.IsDestroyed).ToList();
            Floors = floors.Where(w => !w.IsDestroyed).ToList();

            var wallsFloors = Walls.Cast<Terrain>()
                .Union(Floors)
                .OrderBy(t => t.Index)
                .ToArrayView(Width);

            ApplyTerrainOverlay(wallsFloors);

            return this;
        }

        public MarsMap WithDoors(IEnumerable<Door> doors)
        {
            foreach (var door in doors)
            {
                AddEntity(door);
            }

            return this;
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
            if (obj.GetType() != GetType()) return false;
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

        public Wall CreateWall(WallType wallType, Point position, IGameObjectFactory gameObjectFactory)
        {
            var wall = GetTerrainAt<Wall>(position);

            if (wall != null)
                return wall;

            if (!DestroyFloor(position))
                return null;

            wall = gameObjectFactory.CreateGameObject<Wall>();
            wall.Position = position;
            wall.Index = position.ToIndex(Width);
            wall.WallType = wallType;
            Walls.Add(wall);
            _gameWorld.Walls.Add(wall.ID, wall);

            SetTerrain(wall);
            return wall;
        }

        public Floor CreateFloor(FloorType floorType, Point position, IGameObjectFactory gameObjectFactory)
        {
            var floor = GetTerrainAt<Floor>(position);

            if (floor != null)
                return floor;

            if (!DestroyWall(position))
                return null;

            floor = gameObjectFactory.CreateGameObject<Floor>();
            floor.Position = position;
            floor.Index = position.ToIndex(Width);
            floor.FloorType = floorType;
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
            
            // If KeyNotFoundException happens here then the game object type SaveState or LoadState
            // is not populating the id. Base game object classes populate the id for you (ref1er
            // to existing implementations).
            var gameObjectsOnMap = memento.State.GameObjectIds
                .Select(g => _gameWorld.GameObjects[g])
                .OfType<MarsGameObject>()
                .ToList();

            var terrain = gameObjectsOnMap.OfType<Terrain>().ToList();

            WithTerrain(terrain.OfType<Wall>(), terrain.OfType<Floor>());
                       
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
        
        // Returns whether a point will block movement between different areas of the map
        // if an obstacle is placed there.  Note that technically diagonals could be moved
        // around, but ideally we should exclude placing an item also if the player
        // could only move around it via a diagonal as it does not look pretty.
        public bool IsBlockingIfPlacingObstacle(Point position)
        {
            // get neighbours around position
            // the problem here is that neighbours is not clockwise or anticlockwise.
            // need to get neighbours in clockwise order and also account for edges of map -
            // these could be treated as blocking squares.
            var neighbors = AdjacencyRule.EightWay.NeighborsClockwise(position);
            
            // Every blank square must be reachable by all other blank squares
            // Not blocking:
            // #.#
            // .?.
            // #.#
            //
            // Blocking:
            // ###
            // .?.
            // ###
            //
            // Not blocking:
            // ###
            // .?.
            // ..#
            //
            // Blocking:
            // .##
            // .?.
            // .##
            // Blocking:
            // .#.
            // .?.
            // .#.
            //
            // Blocking:
            // ##.
            // .?#
            // ..#
            var walkablePoints = neighbors
                .Select(WalkabilityOrFalseIfOffMap)
                .Count(p => p);

            // If there's none or one walkable point then it is not blocking
            // because it would be a tunnel end
            if (walkablePoints <= 1)
                return false;

            // Not blocking if more than 6 free squares as there can be no chokepoints
            // if more than 6 free squares
            if (walkablePoints > 6)
                return false;
            
            var rectangle = this.RectangleForRadiusAndPoint(1, position);
            var centre = position - rectangle.MinExtent;
            
            var walkabilitySubset = WalkabilityView.Subset(rectangle);

            // The centre point needs to be marked as false (non-walkable) since we are placing an obstacle there
            walkabilitySubset[centre] = false;
            
            var areaFinder = new MapAreaFinder(walkabilitySubset, AdjacencyRule.EightWay);

            var areas = areaFinder.MapAreas();
            
            // blocking if 2 or more areas
            return areas.Count() >= 2;
        }

        private bool WalkabilityOrFalseIfOffMap(Point point)
        {
            if (!WalkabilityView.Contains(point))
                return false;

            return WalkabilityView[point];
        }

        public Point FindClosestFreeFloor(Point startPoint, bool contiguousFromStartingPoint = true, Func<Point, bool> additionalRule = null)
        {
            if (startPoint == Point.None)
                return startPoint;
            
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
                    {
                        if (additionalRule == null || additionalRule(point))
                            return point;
                    }

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
