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
    
    public PrefabInstance(Prefab prefab, Point location, Direction rotation)
    {
        if (rotation == Direction.Down)
        {
            PrefabText = prefab.PrefabText.Reverse().ToArray();
        }
        else if (rotation == Direction.Right)
        {
            // rotate the string array right
            var rotated = new string[prefab.PrefabText[0].Length];
            
            for (var i = 0; i < rotated.Length; i++)
            {
                for (var j = 0; j < prefab.PrefabText.Length; j++)
                {
                    rotated[i] += prefab.PrefabText[prefab.PrefabText.Length - j - 1][i];
                }
            }
            PrefabText = rotated;
        }
        else if (rotation == Direction.Left)
        {
            // rotate the string array right
            var rotated = new string[prefab.PrefabText[0].Length];
            
            for (var i = 0; i < rotated.Length; i++)
            {
                for (var j = 0; j < prefab.PrefabText.Length; j++)
                {
                    rotated[i] += prefab.PrefabText[prefab.PrefabText.Length - j - 1][i];
                }
            }

            PrefabText = rotated.Reverse().ToArray();
        }
        else
        {
            PrefabText = prefab.PrefabText.ToArray();
        }
            
        Location = location;
        
        var points = GetPointsOfType(PrefabText, c => c != Constants.WallPrefab);
        
        Area = new Area { points };
        Area += Location;

        Id = Guid.NewGuid();
    }

    private List<Point> GetPointsOfType(string[] connectorPrefab, Func<char, bool> func)
    {
        return Prefab.GetPointsOfType(connectorPrefab, func, Location);
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