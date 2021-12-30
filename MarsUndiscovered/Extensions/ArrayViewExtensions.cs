using System;
using System.Collections.Generic;
using System.Linq;

using GoRogue.GameFramework;

using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Extensions
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
    }
}