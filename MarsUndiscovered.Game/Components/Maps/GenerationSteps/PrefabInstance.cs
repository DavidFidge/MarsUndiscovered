using System.Diagnostics;
using SadRogue.Primitives;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components.GenerationSteps;

[DebuggerDisplay("Id: {Id} | Location: {Location}")]
public class PrefabInstance
{
    public string[] PrefabText { get; set; }
    public Prefab Prefab { get; init; }
    public Point Location { get; init; }
    public Area Area { get; init; }

    public PrefabInstance(Prefab prefab, Point location)
    {
        PrefabText = prefab.PrefabText.ToArray();
        Location = location;
        Area = prefab.Area + Location;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }

    public Point GetRandomConnectorPoint(IEnhancedRandom rng)
    {
        var points = GetPointsOfType(Constants.ConnectorPrefab);

        return rng.RandomElement(points);
    }

    public List<Point> GetPointsOfType(char connectorPrefab)
    {
        return Prefab.GetPointsOfType(PrefabText, connectorPrefab, Location);
    }
    
    public char GetPrefabCharAt(Point point)
    {
        var originPoint = point - Location;
        return PrefabText[originPoint.Y][originPoint.X];
    }
}