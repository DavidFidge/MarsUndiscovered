using GoRogue.MapGeneration;
using MarsUndiscovered.Game.Extensions;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.Maps;

public class WallFloorTypeConverterGenerator : GenerationStep
{
    public FloorType FloorType { get; set; } = FloorType.RockFloor;
    public WallType WallType { get; set; } = WallType.RockWall;

    protected override IEnumerator<object> OnPerform(GenerationContext context)
    {
        var wallsFloors = context
            .GetFirst<ArrayView<bool>>(MapGenerator.WallFloorTag)
            .ToArrayView((s, index) => s ? (GameObjectType)FloorType : WallType
            );
        
        context.Add(wallsFloors, MapGenerator.WallFloorTypeTag);

        yield return null;
    }
}