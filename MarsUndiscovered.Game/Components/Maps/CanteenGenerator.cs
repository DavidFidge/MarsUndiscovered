using System.Linq;
using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;

using MarsUndiscovered.Game.Extensions;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.Maps;

public class CanteenGenerator : GenerationStep
{
    protected override IEnumerator<object> OnPerform(GenerationContext context)
    {
        var itemListAreas = context.GetFirst<ItemList<Area>>(MapGenerator.HoleInTheWallAreaTag);

        var wallsFloorTypes = context.GetFirst<ArrayView<GameObjectType>>(MapGenerator.WallFloorTypeTag);

        var wallsFloors = context
            .GetFirst<ArrayView<bool>>(MapGenerator.WallFloorTag);

        var area = itemListAreas.First();

        var wallType = WallType.MiningFacilityWall;
        var floorType = FloorType.MiningFacilityFloor;

        // change all to floor, then change perimeter to wall
        foreach (var point in area.Item)
        {
            wallsFloors[point] = true;
            wallsFloorTypes[point] = floorType;
        }

        var perimeterPositions = area.Item.PerimeterPositions(AdjacencyRule.EightWay);

        foreach (var point in perimeterPositions)
        {
            wallsFloors[point] = false;
            wallsFloorTypes[point] = wallType;
        }

        yield return null;

        // dig out a corridor to the caves and put in a door
         

    }
}