using System.Linq;

using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;

using MarsUndiscovered.Game.Extensions;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.Maps;

// This will not do the digging, it will identify an area.
public class HoleInTheWallGenerator : GenerationStep
{
    protected override IEnumerator<object> OnPerform(GenerationContext context)
    {
        var wallsFloors = context
            .GetFirst<ArrayView<bool>>(MapGenerator.WallFloorInvertedTag);

        // Scan rows and columns for longest contiguous wall
        var z = new
        {
            xOn = 0,
            xOff = 0,
            yOn = 0,
            yOff = 0,
            lastX = wallsFloors[Point.Zero],
            lastY = wallsFloors[Point.Zero]
        }

        for (var x = 0; x < context.Width; x++)
        {
            for (var y = 0; y < context.Height; y++)
            {
                var nextPoint = new Point(x, y);

                if (wallsFloors[nextPoint] == true)
            }
        }

        // Find the area with the largest perimeter (number of perimeter positions)
        var biggestArea = areas
            .OrderByDescending(a => a.PerimeterPositions(AdjacencyRule.Cardinals).Count())
            .FirstOrDefault();

        if (biggestArea is null)
        {
            yield return null;
            yield break;
        }

        // Compute the geometric centroid of the perimeter points (average X and Y)
        var avgX = perimeter.Average(p => p.X);
        var avgY = perimeter.Average(p => p.Y);

        var point = new Point((int)avgX, (int)avgY);

        var lastRectangle = new Rectangle(point, 1, 1);

        var bounds = wallsFloors.Bounds();

        while (true)
        {
            var newRectangle = lastRectangle.Expand(1, 1);

            if (perimeter.Any(p => newRectangle.Contains(p)) 
                || !bounds.Contains(newRectangle))
                break;

            lastRectangle = newRectangle;
        }

        var itemListAreas = context.GetFirstOrNew(() => new ItemList<Area>(), MapGenerator.HoleInTheWallAreaTag);

        var area = new Area(lastRectangle.Positions());

        itemListAreas.Add(area, MapGenerator.HoleInTheWallAreaTag);

        yield return null;
    }
}