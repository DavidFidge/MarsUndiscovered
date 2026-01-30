using FrigidRogue.MonoGame.Core.Extensions;

using GoRogue.GameFramework;

using MarsUndiscovered.Game.Components;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

using ShaiRandom.Collections;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Extensions
{
    public static class ArrayViewExtensions
    {
        public static ArrayView<IGameObject> ToArrayView<T>(this IEnumerable<T> list, int width) where T : IGameObject
        {
            var gameObjects = list.OfType<IGameObject>().ToArray();

            return new ArrayView<IGameObject>(gameObjects, width);
        }

        public static ArrayView<IGameObject> ToArrayView<T>(this IEnumerable<T> list, int width, Func<T, IGameObject> adapter)
        {
            var gameObjects = list.Select(adapter).ToArray();

            return new ArrayView<IGameObject>(gameObjects, width);
        }

        public static ArrayView<IGameObject> ToArrayView<T>(this IEnumerable<T> list, int width, Func<T, int, IGameObject> adapter)
        {
            var gameObjects = list.Select(adapter).ToArray();

            return new ArrayView<IGameObject>(gameObjects, width);
        }

        public static ArrayView<IGameObject> ToArrayView<T>(this ArrayView<T> list, Func<T, IGameObject> adapter)
        {
            return ToArrayView(list.ToArray(), list.Width, adapter);
        }

        public static ArrayView<IGameObject> ToArrayView<T>(this ArrayView<T> list, Func<T, int, IGameObject> adapter)
        {
            return ToArrayView(list.ToArray(), list.Width, adapter);
        }

        public static ArrayView<TResult> ToArrayView<T, TResult>(this IEnumerable<T> list, int width, Func<T, TResult> adapter)
        {
            var gameObjects = list.Select(adapter).ToArray();

            return new ArrayView<TResult>(gameObjects, width);
        }

        public static ArrayView<TResult> ToArrayView<T, TResult>(this IEnumerable<T> list, int width, Func<T, int, TResult> adapter)
        {
            var gameObjects = list.Select(adapter).ToArray();

            return new ArrayView<TResult>(gameObjects, width);
        }

        public static ArrayView<TResult> ToArrayView<T, TResult>(this ArrayView<T> list, Func<T, TResult> adapter)
        {
            return ToArrayView(list.ToArray(), list.Width, adapter);
        }

        public static ArrayView<TResult> ToArrayView<T, TResult>(this ArrayView<T> list, Func<T, int, TResult> adapter)
        {
            return ToArrayView(list.ToArray(), list.Width, adapter);
        }
        
        public static ArrayView<TResult> ToArrayView<T, TResult>(this IGridView<T> list, Func<T, TResult> adapter)
        {
            if (list is ArrayView<T> a)
            {
                return ToArrayView(a, adapter);
            }
            
            var arrayView = new ArrayView<T>(list.Width, list.Height);
            arrayView.ApplyOverlay(c => list[c]);

            return ToArrayView(arrayView, adapter);
        }

        public static bool HasNeighbouringFloorVerticallyOrHorizontallyWithFloorTypeCheck(this Point point, ArrayView<GameObjectType> wallsFloorTypes)
        {
            var width = wallsFloorTypes.Width;
            var height = wallsFloorTypes.Height;
            
            var neighbours = point.Neighbours(width - 1, height - 1);
        
            if ((neighbours.Contains(point + Direction.Up) && wallsFloorTypes[(point + Direction.Up).ToIndex(width)] is FloorType) &&
                (neighbours.Contains(point + Direction.Down) && wallsFloorTypes[(point + Direction.Down).ToIndex(width)] is FloorType))
            {
                return true;
            }
        
            if ((neighbours.Contains(point + Direction.Left) && wallsFloorTypes[(point + Direction.Left).ToIndex(width)] is FloorType) &&
                (neighbours.Contains(point + Direction.Right) && wallsFloorTypes[(point + Direction.Right).ToIndex(width)] is FloorType))
            {
                return true;
            }

            return false;
        }
        
        public static bool HasNeighbouringFloorVerticallyOrHorizontally<T>(this Point point, ArrayView<T> map)
        {
            var width = map.Width;
            var height = map.Height;
            
            var neighbours = point.Neighbours(width - 1, height - 1);
        
            if (neighbours.Contains(point + Direction.Up) &&
                neighbours.Contains(point + Direction.Down))
            {
                return true;
            }
        
            if (neighbours.Contains(point + Direction.Left) &&
                neighbours.Contains(point + Direction.Right))
            {
                return true;
            }

            return false;
        }
    }
}
