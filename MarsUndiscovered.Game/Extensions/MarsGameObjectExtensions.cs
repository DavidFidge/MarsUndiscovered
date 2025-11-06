using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;

namespace MarsUndiscovered.Game.Extensions
{
    public static class MarsGameObjectExtensions
    {
        public static bool IsGameObjectObstacle(this IGameObject gameObject)
        {
            switch (gameObject)
            {
                case Player _: //??????
                case Monster _:
                case Indestructible _:
                case Wall _:
                    return true;
                default:
                    return false;
            }
        }
        
        public static bool IsGameObjectStrikeThrough(this IGameObject gameObject)
        {
            switch (gameObject)
            {
                case Monster _:
                case Player _:
                    return true;
                default:
                    return !gameObject.IsGameObjectObstacle();
            }
        }
    }
}