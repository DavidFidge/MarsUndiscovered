using System.Linq;

using Castle.Components.DictionaryAdapter.Xml;

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
            .GetFirst<ArrayView<bool>>(MapGenerator.WallFloorTag);

        // Scan rows and columns for longest contiguous wall
        var contiguous = new ContiguousWallFinder();
        contiguous.Execute(wallsFloors, wallsFloors.Bounds().ChangeSize(new Point(-8, -8)).ChangePosition(new Point(4, 4)));

        var triedPoints = new List<Point>();

        while (true)
        {
            var point = contiguous.LongestXYIntersect(10, triedPoints);

            if (point == Point.None)
                throw new RegenerateMapException();

            var lastRectangle = new Rectangle(point, 1, 1);

            var bounds = wallsFloors.Bounds();

            var validExpansionDirections = new Dictionary<Direction, bool>();
            validExpansionDirections.Add(Direction.Left, true);
            validExpansionDirections.Add(Direction.Right, true);
            validExpansionDirections.Add(Direction.Up, true);
            validExpansionDirections.Add(Direction.Down, true);

            while (validExpansionDirections.Values.Any(d => d == true) && lastRectangle.Height < 12 && lastRectangle.Width < 12)
            {
                var newRectangle = lastRectangle;

                if (validExpansionDirections[Direction.Left])
                {
                    newRectangle = newRectangle.ChangePosition(new Point(-1, 0)).ChangeWidth(1);

                    if (!BoundsValid(newRectangle, bounds, wallsFloors))
                    {
                        validExpansionDirections[Direction.Left] = false;
                        newRectangle = lastRectangle;
                    }
                    else
                    {
                        lastRectangle = newRectangle;
                    }
                }

                if (validExpansionDirections[Direction.Right])
                {
                    newRectangle = newRectangle.ChangeWidth(1);

                    if (!BoundsValid(newRectangle, bounds, wallsFloors))
                    {
                        validExpansionDirections[Direction.Right] = false;
                        newRectangle = lastRectangle;
                    }
                    else
                    {
                        lastRectangle = newRectangle;
                    }
                }

                if (validExpansionDirections[Direction.Up])
                {
                    newRectangle = newRectangle.ChangePosition(new Point(0, -1)).ChangeHeight(1);

                    if (!BoundsValid(newRectangle, bounds, wallsFloors))
                    {
                        validExpansionDirections[Direction.Up] = false;
                        newRectangle = lastRectangle;
                    }
                    else
                    {
                        lastRectangle = newRectangle;
                    }
                }

                newRectangle = lastRectangle;

                if (validExpansionDirections[Direction.Down])
                {
                    newRectangle = newRectangle.ChangeHeight(1);

                    if (!BoundsValid(newRectangle, bounds, wallsFloors))
                    {
                        validExpansionDirections[Direction.Down] = false;
                    }
                    else
                    {
                        lastRectangle = newRectangle;
                    }
                }
            }

            // Try next intersection if this one is not big enough
            if (lastRectangle.Width < 4 || lastRectangle.Height < 4)
            {
                triedPoints.Add(point);
                continue;
            }

            var itemListAreas = context.GetFirstOrNew(() => new ItemList<Area>(), MapGenerator.HoleInTheWallAreaTag);

            var area = new Area(lastRectangle.Positions());

            itemListAreas.Add(area, MapGenerator.HoleInTheWallAreaTag);

            break;
        }

        yield return null;
    }

    private bool BoundsValid(Rectangle rectangle, Rectangle bounds, ArrayView<bool> wallsFloors)
    {
        if (!bounds.Contains(rectangle))
            return false;

        var perimeter = rectangle.PerimeterPositions();

        // disallow going over existing floors
        if (perimeter.Any(p => wallsFloors[p] == true))
            return false;

        return true;
    }
}