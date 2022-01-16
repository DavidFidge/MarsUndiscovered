using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoRogue.GameFramework;
using MonoGame.Extended;
using SadRogue.Primitives;

namespace MarsUndiscovered.Components
{
    public static class MapHelpers
    {
        public static Func<Point, IEnumerable<IGameObject>, bool> PointOnFloor = (p, gameObjects) =>
            gameObjects.Any(g => g is Floor);

        public static Func<Point, IEnumerable<IGameObject>, bool> EmptyPointOnFloor = (p, gameObjects) =>
            PointOnFloor(p, gameObjects) && !gameObjects.Any(g => g is Actor || g is Item);
    }
}
