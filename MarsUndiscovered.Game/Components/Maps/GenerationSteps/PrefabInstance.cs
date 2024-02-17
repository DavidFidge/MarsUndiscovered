using System.Diagnostics;
using FrigidRogue.MonoGame.Core.Extensions;
using Microsoft.VisualBasic;
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
    
    public PrefabInstance(Prefab prefab, Point location, Direction rotation, bool mirrored = false)
    {
        Prefab = prefab;

        PrefabText = prefab.PrefabText.ToArray();
        
        if (rotation == Direction.Up)
        {
            if (mirrored)
            {
                PrefabText = PrefabText
                    .Select(t => t.ReverseString())
                    .ToArray();
            }
        }
        if (rotation == Direction.Down)
        {
            if (!mirrored)
            {
                PrefabText = PrefabText
                    .Select(t => t.ReverseString())
                    .ToArray();
            }
            
            PrefabText = PrefabText.Reverse().ToArray();
        }
        else if (rotation == Direction.Left)
        {
            if (mirrored)
            {
                PrefabText = PrefabText
                    .Select(t => t.ReverseString())
                    .ToArray();
            }
            
            var rotated = new string[PrefabText[0].Length];
            
            for (var i = rotated.Length - 1; i >= 0; i--)
            {
                for (var j = 0; j < PrefabText.Length; j++)
                {
                    rotated[rotated.Length - 1 - i] += PrefabText[j][i];
                }
            }
            
            PrefabText = rotated;
        }
        else if (rotation == Direction.Right)
        {
            // rotate the string array right
            if (mirrored)
            {
                PrefabText = PrefabText
                    .Select(t => t.ReverseString())
                    .ToArray();
            }
            
            var rotated = new string[PrefabText[0].Length];
            
            for (var i = 0; i < rotated.Length; i++)
            {
                for (var j = PrefabText.Length - 1; j >= 0; j--)
                {
                    rotated[i] += PrefabText[j][i];
                }
            }
            
            PrefabText = rotated;
        }
            
        Location = location;
        
        var points = GetPointsOfType(PrefabText, c => c != Constants.WallPrefab);
        
        Area = new Area { points };

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