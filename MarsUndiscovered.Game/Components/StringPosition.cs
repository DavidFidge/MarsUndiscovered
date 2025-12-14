
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components;

public class NamePosition
{
    public NamePosition(string name, Point position)
    {
        Name = name;
        Position = position;
    }
    
    public string Name { get; set; }
    public Point Position { get; set; }
}