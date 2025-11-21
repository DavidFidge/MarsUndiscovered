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
            .GetFirst<ArrayView<bool>>(MapGenerator.WallFloorInvertedTag);

        // Scan rows and columns for longest contiguous wall
        var contiguous = new ContiguousWallFinder();

        contiguous.Execute(wallsFloors);
        var point = contiguous.LongestXYIntersect();

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