using GoRogue.MapGeneration;
using MarsUndiscovered.Game.Extensions;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.Maps;

public class WallFloorInverterGenerator : GenerationStep
{
    protected override IEnumerator<object> OnPerform(GenerationContext context)
    {
        var wallsFloors = context
            .GetFirst<ArrayView<bool>>(MapGenerator.WallFloorInvertedTag)
            .ToArrayView((s, index) => !s);
        
        context.Add(wallsFloors, MapGenerator.WallFloorTypeTag);

        yield return null;
    }
}