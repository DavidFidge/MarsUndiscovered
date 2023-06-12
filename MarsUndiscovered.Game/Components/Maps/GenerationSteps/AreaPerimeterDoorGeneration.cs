using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using GoRogue.Random;
using MarsUndiscovered.Game.Components.Maps;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components.GenerationSteps;

public class AreaPerimeterDoorGeneration : GenerationStep
{
    private readonly FloorType _floorType;
    private readonly DoorType _doorType;
    private readonly int _minDoors;
    private readonly int _maxDoors;
    public string AreasComponentTag { get; set; } = MapGenerator.AreasTag;
    public string AreasStepFilterTag { get; set; } = null;
    public string DoorsTag { get; set; } = MapGenerator.DoorsTag;
    public string WallFloorTypeTag { get; set; } = MapGenerator.WallFloorTypeTag;

    public IEnhancedRandom RNG { get; set; } = GlobalRandom.DefaultRNG;

    public AreaPerimeterDoorGeneration(FloorType floorType, DoorType doorType, string name = null, int minDoors = 1, int maxDoors = 1)
        : base(name)
    {
        _floorType = floorType;
        _doorType = doorType;
        _minDoors = minDoors;
        _maxDoors = maxDoors;
    }
    
    private bool HasNeighbouringFloorVerticallyOrHorizontally(Point point, ArrayView<GameObjectType> wallsFloorTypes, int width, int height)
    {
        var neighbours = point.Neighbours(width - 1, height - 1);
        
        if ((neighbours.Contains(point + Direction.Up) && wallsFloorTypes[(point + Direction.Up).ToIndex(width)] is FloorType) &&
            (neighbours.Contains(point + Direction.Down) && wallsFloorTypes[(point + Direction.Down).ToIndex(width)] is FloorType))
        {
            return true;
        }
        
        if ((neighbours.Contains(point + Direction.Left) && wallsFloorTypes[(point + Direction.Left).ToIndex(width)] is FloorType) &&
            (neighbours.Contains(point + Direction.Right) && wallsFloorTypes[(point + Direction.Right).ToIndex(width)] is FloorType))
        {
            return true;
        }

        return false;
    }

    protected override IEnumerator<object> OnPerform(GenerationContext context)
    {
        var areas = context
            .GetFirst<ItemList<Area>>(AreasComponentTag)
            .Where(a => String.IsNullOrEmpty(AreasStepFilterTag) || a.Step == AreasStepFilterTag)
            .ToList();

        var doors = context.GetFirstOrNew(() => new ItemList<GameObjectTypePosition<DoorType>>(), DoorsTag);

        var wallsFloorTypes = context.GetFirst<ArrayView<GameObjectType>>(WallFloorTypeTag);
        
        var doorsToCreate = RNG.NextInt(_minDoors, _maxDoors + 1);

        if (doorsToCreate > 0)
        {
            foreach (var area in areas.Select(a => a.Item))
            {
                var validPerimeterPoints = area.PerimeterPositions(AdjacencyRule.EightWay)
                    .Where(p => wallsFloorTypes[p.ToIndex(context.Width)] is WallType)
                    .Where(p => HasNeighbouringFloorVerticallyOrHorizontally(p, wallsFloorTypes, context.Width,
                        context.Height))
                    .ToList();

                while (doorsToCreate > 0 && validPerimeterPoints.Any())
                {
                    var newDoorIndex = RNG.RandomIndex(validPerimeterPoints);

                    var newDoorPoint = validPerimeterPoints[newDoorIndex];

                    validPerimeterPoints.RemoveAt(newDoorIndex);

                    wallsFloorTypes[newDoorPoint.ToIndex(context.Width)] = _floorType;

                    doors.Add(new GameObjectTypePosition<DoorType>(_doorType, newDoorPoint), Name);

                    doorsToCreate--;
                }
            }
        }

        yield return null;
    }
}