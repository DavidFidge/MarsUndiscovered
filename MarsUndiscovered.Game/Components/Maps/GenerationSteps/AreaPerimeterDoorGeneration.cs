using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using GoRogue.Random;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Game.Extensions;
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
    
    protected override IEnumerator<object> OnPerform(GenerationContext context)
    {
        var areas = context
            .GetFirst<ItemList<Area>>(AreasComponentTag)
            .Where(a => String.IsNullOrEmpty(AreasStepFilterTag) || a.Step == AreasStepFilterTag)
            .ToList();

        var doors = context.GetFirstOrNew(() => new ItemList<GameObjectTypePosition<DoorType>>(), DoorsTag);

        var wallsFloorTypes = context.GetFirst<ArrayView<GameObjectType>>(WallFloorTypeTag);

        foreach (var area in areas.Select(a => a.Item))
        {
            var validPerimeterPoints = area.PerimeterPositions(AdjacencyRule.EightWay)
                .Where(p => wallsFloorTypes[p.ToIndex(context.Width)] is WallType)
                .Where(p => p.HasNeighbouringFloorVerticallyOrHorizontallyWithFloorTypeCheck(wallsFloorTypes))
                .ToList();

            var doorsToCreate = RNG.NextInt(_minDoors, _maxDoors + 1);

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
        
        yield return null;
    }
}