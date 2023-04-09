using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Extensions
{
    public static class MarsGameObjectFluentExtensions
    {
        public static T PositionedAt<T>(this T gameObject, Point position) where T : MarsGameObject
        {
            gameObject.Position = position;
            return gameObject;
        }

        public static T PositionedAt<T>(this T gameObject, Func<T, Point> position) where T : MarsGameObject
        {
            gameObject.Position = position(gameObject);
            return gameObject;
        }

        public static T AddToMap<T>(this T gameObject, Map map) where T : MarsGameObject
        {
            map.AddEntity(gameObject);
            return gameObject;
        }
    }
}