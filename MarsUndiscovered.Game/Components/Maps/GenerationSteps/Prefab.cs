using SadRogue.Primitives;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components.GenerationSteps;

public class Prefab
{
    public string[] PrefabText { get; set; }
    public Rectangle Bounds => new Rectangle(0, 0, PrefabText[0].Length, PrefabText.Length);

    public Point GetRandomConnectorPoint(IEnhancedRandom rng)
    {
        var points = GetPointsOfType(Constants.ConnectorPrefab);

        return rng.RandomElement(points);
    }

    private List<Point> GetPointsOfType(char c)
    {
        var connectorPoints = new List<Point>();
        for (var y = 0; y < PrefabText.Length; y++)
        {
            for (var x = 0; x < PrefabText[y].Length; x++)
            {
                if (PrefabText[y][x] == c)
                {
                    connectorPoints.Add(new Point(x, y));
                }
            }
        }

        return connectorPoints;
    }
}