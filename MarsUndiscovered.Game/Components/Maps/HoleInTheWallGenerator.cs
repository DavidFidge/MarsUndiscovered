using GoRogue.MapGeneration;

using MarsUndiscovered.Game.Extensions;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.Maps;

public class HoleInTheWallGenerator : GenerationStep
{
    protected override IEnumerator<object> OnPerform(GenerationContext context)
    {
        var wallsFloors = context
            .GetFirst<ArrayView<bool>>(MapGenerator.WallFloorInvertedTag);

        var areaFinder = new MapAreaFinder(wallsFloors, AdjacencyRule.EightWay);

        var areas = areaFinder.MapAreas();

        var biggestArea = areas.Max(a => a.PerimeterPositions(AdjacencyRule.Cardinals));

        var rectangle = new Rectangle(


        yield return null;
    }
}