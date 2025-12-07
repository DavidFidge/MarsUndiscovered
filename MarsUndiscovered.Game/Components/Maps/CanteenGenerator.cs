using System.Linq;

using FrigidRogue.MonoGame.Core.Extensions;

using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using GoRogue.Random;

using MarsUndiscovered.Game.Extensions;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components.Maps;

public class CanteenGenerator : GenerationStep
{
    public IEnhancedRandom RNG { get; set; } = GlobalRandom.DefaultRNG;

    protected override IEnumerator<object> OnPerform(GenerationContext context)
    {
        var itemListAreas = context.GetFirst<ItemList<Area>>(MapGenerator.HoleInTheWallRectangleTag);

        var wallsFloorTypes = context.GetFirst<ArrayView<GameObjectType>>(MapGenerator.WallFloorTypeTag);

        var wallsFloors = context
            .GetFirst<ArrayView<bool>>(MapGenerator.WallFloorTag);

        var area = itemListAreas.First();

        var wallTypeCanteen = WallType.MiningFacilityWall;
        var floorTypeCanteen = FloorType.MiningFacilityFloor;
        var floorTypeRock = FloorType.RockFloor;

        // change all to floor, then change perimeter to wall
        foreach (var point in area.Item)
        {
            wallsFloors[point] = true;
            wallsFloorTypes[point] = floorTypeCanteen;
        }

        var perimeterPositions = area.Item.PerimeterPositions(AdjacencyRule.EightWay).ToList();

        foreach (var point in perimeterPositions)
        {
            wallsFloors[point] = false;
            wallsFloorTypes[point] = wallTypeCanteen;
        }

        yield return null;

        // dig out a corridor to the caves and put in a door
        var success = false;
        var tries = 0;

        while (!success && tries < 100)
        {
            tries++;
            var randomPerimeterPosition = RNG.RandomElement(perimeterPositions);

            // reject any points on the map boundary
            if (!wallsFloors.Contains(randomPerimeterPosition.Add(Direction.Down))
                || !wallsFloors.Contains(randomPerimeterPosition.Add(Direction.Up))
                || !wallsFloors.Contains(randomPerimeterPosition.Add(Direction.Left))
                || !wallsFloors.Contains(randomPerimeterPosition.Add(Direction.Right)))
                continue;

            var seekDirection = Direction.None;

            // find a floor that is adjacent to the perimeter point.
            // this is most likely in the canteen, though it may not be
            // (but it doesn't really matter since our dig will still
            // make it accessible).
            if (wallsFloors[randomPerimeterPosition.Add(Direction.Down)] == true)
                seekDirection = Direction.Up;
            else if (wallsFloors[randomPerimeterPosition.Add(Direction.Up)] == true)
                seekDirection = Direction.Down;
            else if (wallsFloors[randomPerimeterPosition.Add(Direction.Left)] == true)
                seekDirection = Direction.Right;
            else if (wallsFloors[randomPerimeterPosition.Add(Direction.Right)] == true)
                seekDirection = Direction.Left;

            // reject if seek direction is none
            if (seekDirection == Direction.None)
                continue;

            // check the opposite direction point, if is blocked by a wall then reject - it is
            // likely we are on a corner of the canteen and the free side happened to be a free wall.
            if (wallsFloors[randomPerimeterPosition.Add(seekDirection.Opposite())] == false)
                continue;

            var tunnelSuccess = false;
            var tunnel = new List<Point>();
            var nextPoint = randomPerimeterPosition;

            while (true)
            {
                nextPoint += seekDirection;

                if (!wallsFloors.Contains(nextPoint))
                    break;

                if (wallsFloors[nextPoint] == true)
                {
                    tunnelSuccess = true;
                    break;
                }

                tunnel.Add(nextPoint);
            }

            if (!tunnelSuccess)
                continue;

            success = true;

            foreach (var tunnelPoint in tunnel)
            {
                wallsFloors[tunnelPoint] = true;
                wallsFloorTypes[tunnelPoint] = floorTypeRock;
            }

            wallsFloors[randomPerimeterPosition] = true;
            wallsFloorTypes[randomPerimeterPosition] = floorTypeCanteen;

            var doors = context.GetFirstOrNew(() => new ItemList<GameObjectTypePosition<DoorType>>(), MapGenerator.DoorsTag);

            doors.Add(new GameObjectTypePosition<DoorType>(DoorType.DefaultDoor, randomPerimeterPosition), Name);

            var waypoints = context.GetFirstOrNew(() => new ItemList<NamePosition>(), MapGenerator.WaypointTag);

            waypoints.Add(new NamePosition(Constants.WaypointCanteen, area.Item.GetMidpoint()), Name);

            yield return null;
        }

        if (!success)
            throw new RegenerateMapException();
    }
}