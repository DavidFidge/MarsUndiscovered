using GoRogue.MapGeneration;
using MarsUndiscovered.Game.Extensions;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.Maps;

public class WallFloorTypeConverterGenerator : GenerationStep
{
    protected override IEnumerator<object> OnPerform(GenerationContext context)
    {
        var wallsFloors = context
            .GetFirst<ArrayView<bool>>(MapGenerator.WallFloorTag)
            .ToArrayView((s, index) => s ? (GameObjectType)FloorType.RockFloor : WallType.RockWall
            );
        
        context.Add(wallsFloors, MapGenerator.WallFloorTypeTag);

        yield return null;
    }
}