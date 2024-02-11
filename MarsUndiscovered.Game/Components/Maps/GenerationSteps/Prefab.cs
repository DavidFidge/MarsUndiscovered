using SadRogue.Primitives;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components.GenerationSteps;

public class Prefab
{
    private readonly Area _area;
    public Area Area => _area;
    public string[] PrefabText { get; init; }
    public Rectangle Bounds => new Rectangle(0, 0, PrefabText[0].Length, PrefabText.Length);

    public Prefab(string[] prefabText)
    {
        PrefabText = prefabText;
        var points = GetPointsOfType(PrefabText, c => c != Constants.WallPrefab);
        _area = new Area();
        _area.Add(points);
    }
    
    public static List<Point> GetPointsOfType(string[] prefabText, char c)
    {
        return GetPointsOfType(prefabText, x => x == c, Point.Zero);
    }
    
    public static List<Point> GetPointsOfType(string[] prefabText, char c, Point offset)
    {
        return GetPointsOfType(prefabText, x => x == c, offset);
    }
    
    public static List<Point> GetPointsOfType(string[] prefabText, Func<char, bool> predicate)
    {
        return GetPointsOfType(prefabText, predicate, Point.Zero);
    }

    public static List<Point> GetPointsOfType(string[] prefabText, Func<char, bool> predicate, Point offset)
    {
        if (offset == Point.None)
            offset = Point.Zero;
        
        var connectorPoints = new List<Point>();
        for (var y = 0; y < prefabText.Length; y++)
        {
            for (var x = 0; x < prefabText[y].Length; x++)
            {
                if (predicate(prefabText[y][x]))
                {
                    connectorPoints.Add(new Point(x, y) + offset);
                }
            }
        }

        return connectorPoints;
    }
}