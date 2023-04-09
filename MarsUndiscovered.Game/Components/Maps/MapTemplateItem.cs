using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.Maps;

public struct MapTemplateItem
{
    public char Char { get; }
    public Point Point { get; }

    public MapTemplateItem(char c, Point point)
    {
        Char = @c;
        Point = point;
    }
}