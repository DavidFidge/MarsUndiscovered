
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components;

public class GameObjectTypePosition<T> where T : GameObjectType
{
    public GameObjectTypePosition(T gameObjectType, Point position)
    {
        GameObjectType = gameObjectType;
        Position = position;
    }
    
    public T GameObjectType { get; set; }
    public Point Position { get; set; }
}