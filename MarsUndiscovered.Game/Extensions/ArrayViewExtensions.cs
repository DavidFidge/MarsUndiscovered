using GoRogue.GameFramework;

using SadRogue.Primitives.GridViews;

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
    }
}
