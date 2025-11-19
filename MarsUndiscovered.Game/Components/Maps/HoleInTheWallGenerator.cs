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

        var areaFinder = new MapAreaFinder(wallsFloors, AdjacencyRule.EightWay);

        var areas = areaFinder.MapAreas();

        // Find the area with the largest perimeter (number of perimeter positions)
        var biggestArea = areas
            .OrderByDescending(a => a.PerimeterPositions(AdjacencyRule.Cardinals).Count())
            .FirstOrDefault();

        if (biggestArea is null)
        {
            yield return null;
            yield break;
        }

        // Get the perimeter positions as a list
        var perimeter = biggestArea.PerimeterPositions(AdjacencyRule.Cardinals).ToList();

        if (perimeter.Count == 0)
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