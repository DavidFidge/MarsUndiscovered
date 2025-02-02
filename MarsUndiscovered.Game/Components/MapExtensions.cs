using GoRogue.GameFramework;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components
{
    public static class MapExtensions
    {
        public static bool AnyBlockingObjectsAt(this Map map, Point position)
        {
            return map.GetObjectsAt(position).Any(o => !o.IsWalkable);
        }

        public static IList<IGameObject> GetBlockingObjectsAt(this Map map, Point position)
        {
            return map.GetObjectsAt(position)
                .Where(o => !o.IsWalkable)
                .ToList();
        }

        public static IList<T> GetTerrainAround<T>(this Map map, Point position, AdjacencyRule adjacencyRule) where T : Terrain
        {
            return adjacencyRule.Neighbors(position)
                .Where(p => map.Bounds().Contains(p))
                .Select(map.GetTerrainAt<T>)
                .Where(s => s != null)
                .ToList();
        }

        public static IList<T> GetObjectsAround<T>(this Map map, Point position, AdjacencyRule adjacencyRule) where T : class, IGameObject
        {
            return adjacencyRule.Neighbors(position)
                .Where(p => map.Bounds().Contains(p))
                .SelectMany(p => map.GetObjectsAt<T>(p))
                .ToList();
        }

        public static IList<IGameObject> GetObjectsAround(this Map map, Point position, AdjacencyRule adjacencyRule)
        {
            return adjacencyRule.Neighbors(position)
                .Where(p => map.Bounds().Contains(p))
                .SelectMany(p => map.GetObjectsAt(p))
                .ToList();
        }
        
        public static MarsMap MarsMap(this Map map)
        {
            return (MarsMap)map;
        }
    }
}
