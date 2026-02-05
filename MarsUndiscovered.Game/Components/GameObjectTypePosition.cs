
using System.Dynamic;

using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components;

public class GameObjectTypePosition<T> where T : GameObjectType
{
    public GameObjectTypePosition(T gameObjectType, Point position)
    {
        GameObjectType = gameObjectType;
        Position = position;
        Data = new ExpandoObject();
    }
    
    public T GameObjectType { get; set; }
    public Point Position { get; set; }
    public dynamic Data { get; set; }
}