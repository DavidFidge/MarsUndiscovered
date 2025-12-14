using GoRogue.MapGeneration;
using MarsUndiscovered.Game.Extensions;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.Maps;

public class WallFloorInverterGenerator : GenerationStep
{
    protected override IEnumerator<object> OnPerform(GenerationContext context)
    {
        var wallsFloorsInvertedFunc = () => context
            .GetFirst<ArrayView<bool>>(MapGenerator.WallFloorTag)
            .ToArrayView((s, index) => !s);

        var existingWallsFloorsInverted = context
            .GetFirstOrNew<ArrayView<bool>>(wallsFloorsInvertedFunc, MapGenerator.WallFloorInvertedTag);

        existingWallsFloorsInverted.ApplyOverlay(wallsFloorsInvertedFunc());

        yield return null;
    }
}