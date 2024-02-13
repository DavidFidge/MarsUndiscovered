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
        
        if (rotation == Direction.Down)
        {
            // this rotates and mirrors
            PrefabText = prefab.PrefabText.Reverse().ToArray();

            if (!mirrored)
            {
                PrefabText = PrefabText
                    .Select(t => t.ReverseString())
                    .ToArray();
            }
        }
        else if (rotation == Direction.Right)
        {
            PrefabText = prefab.PrefabText;
            
            if (mirrored)
            {
                PrefabText = PrefabText
                    .Select(t => t.ReverseString())
                    .ToArray();
            }
            
            // rotate the string array right
            var rotated = new string[PrefabText[0].Length];
            
            for (var i = 0; i < rotated.Length; i++)
            {
                for (var j = 0; j < PrefabText.Length; j++)
                {
                    rotated[i] += PrefabText[PrefabText.Length - j - 1][i];
                }
            }
            
            PrefabText = rotated;
        }
        else if (rotation == Direction.Left)
        {
            // Rotate Left
            PrefabText = prefab.PrefabText;
            
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
                    rotated[i] += PrefabText[j][PrefabText[0].Length - i - 1];
                }
            }

            PrefabText = rotated;
        }
        else
        {
            PrefabText = prefab.PrefabText;
            
            if (mirrored)
            {
                PrefabText = PrefabText
                    .Select(t => t.ReverseString())
                    .ToArray();
            }
            else
            {
                PrefabText = PrefabText.ToArray();
            }
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