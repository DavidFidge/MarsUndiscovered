﻿using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using GoRogue.Random;
using MarsUndiscovered.Game.Components.Maps;
using NGenerics.Extensions;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Collections;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components.GenerationSteps;

public class InternalWallsGeneration : GenerationStep
{
    private readonly WallType _wallType;
    private readonly DoorType _doorType;
    private readonly ProbabilityTable<int> _doorProbabilityTable;
    private readonly int _splitFactor;
    public string AreasComponentTag { get; set; } = MapGenerator.AreasTag;
    public string AreasStepFilterTag { get; set; } = null;
    public string AreaWallsDoorsTag { get; set; } = MapGenerator.AreasWallsDoorsTag;
    public string DoorsTag { get; set; } = MapGenerator.DoorsTag;

    public IEnhancedRandom RNG { get; set; } = GlobalRandom.DefaultRNG;

    public InternalWallsGeneration(WallType wallType, DoorType doorType, ProbabilityTable<int> doorProbabilityTable = null, string name = null, int splitFactor = 5)
        : base(name)
    {
        _wallType = wallType;
        _doorType = doorType;
        _splitFactor = splitFactor;
        _doorProbabilityTable = doorProbabilityTable;

        if (_doorProbabilityTable == null)
        {
            var probabilityTable = new ProbabilityTable<int>(new List<(int item, double weight)> { (1, 1) });
            _doorProbabilityTable = probabilityTable;
        }
    }

    protected override IEnumerator<object> OnPerform(GenerationContext context)
    {
        _doorProbabilityTable.Random = RNG;

        var areas = context
            .GetFirstOrNew(() => new ItemList<Area>(), AreasComponentTag)
            .Where(a => String.IsNullOrEmpty(AreasStepFilterTag) || a.Step == AreasStepFilterTag)
            .ToList();

        var areaWallsDoors = context.GetFirstOrNew(() => new ItemList<AreaWallsDoors>(), AreaWallsDoorsTag);
        var doors = context.GetFirstOrNew(() => new ItemList<GameObjectTypePosition<DoorType>>(), MapGenerator.DoorsTag);

        var wallsFloorTypes = context.GetFirst<ArrayView<GameObjectType>>(MapGenerator.WallFloorTypeTag);

        if (!areas.Any())
        {
            // If no pre-existing area has been found then just use the whole map
            var area = new Area(wallsFloorTypes.Bounds().Positions());
            areas.Add(new ItemStepPair<Area>(area, String.Empty));
        }

        foreach (var area in areas.Select(a => a.Item))
        {
            var newDoors = new List<Point>();
            var newWalls = new List<Point>();

            SplitAreaRecursive(area, newWalls, newDoors);

            areaWallsDoors.Add(
                new AreaWallsDoors
                {
                    Area = area,
                    Walls = newWalls,
                    Doors = newDoors
                },
                Name
                );

            foreach (var wall in newWalls)
            {
                wallsFloorTypes[wall.ToIndex(context.Width)] = _wallType;
            }
            
            foreach (var door in newDoors)
            {
                doors.Add(new GameObjectTypePosition<DoorType>(_doorType, door), Name);
            }
        }
        
        yield return null;
    }

    private void SplitAreaRecursive(Area area, List<Point> walls, List<Point> doors)
    {
        var splitVertical = area.Bounds.Width > area.Bounds.Height;
        var splitLength = splitVertical ? area.Bounds.Width : area.Bounds.Height;

        if (splitLength < _splitFactor)
            return;

        List<Point> newWallPoints;
        Area splitArea1;
        Area splitArea2;

        if (splitVertical)
        {
            var validSplit = new List<int>(area.Bounds.MaxExtentX - area.Bounds.MinExtentX);

            for (var x = area.Bounds.MinExtentX + 1; x <= area.Bounds.MaxExtentX - 1; x++)
            {
                var disallowedDoorPointTop = new Point(x, area.Bounds.MinExtentY - 1);
                var disallowedDoorPointBottom = new Point(x, area.Bounds.MaxExtentY + 1);

                if (!doors.Any(d => d.Equals(disallowedDoorPointTop) || d.Equals(disallowedDoorPointBottom)))
                {
                    validSplit.Add(x);
                }
            }

            if (validSplit.IsEmpty())
                return;
            
            var splitPoint = RNG.RandomElement(validSplit);

            newWallPoints = area.Where(p => p.X == splitPoint).ToList();
            splitArea1 = new Area(area.Where(p => p.X < splitPoint));
            splitArea2 = new Area(area.Where(p => p.X > splitPoint));
        }
        else
        {
            // do the same as if condition above but for y
            var validSplit = new List<int>(area.Bounds.MaxExtentY - area.Bounds.MinExtentY);

            for (var y = area.Bounds.MinExtentY + 1; y <= area.Bounds.MaxExtentY - 1; y++)
            {
                var disallowedDoorPointLeft = new Point(area.Bounds.MinExtentX - 1, y);
                var disallowedDoorPointRight = new Point(area.Bounds.MaxExtentX + 1, y);

                if (!doors.Any(d => d.Equals(disallowedDoorPointLeft) || d.Equals(disallowedDoorPointRight)))
                {
                    validSplit.Add(y);
                }
            }

            if (validSplit.IsEmpty())
                return;

            var splitPoint = RNG.RandomElement(validSplit);
            
            newWallPoints = area.Where(p => p.Y == splitPoint).ToList();
            splitArea1 = new Area(area.Where(p => p.Y < splitPoint));
            splitArea2 = new Area(area.Where(p => p.Y > splitPoint));
        }

        var numDoorsToCreate = _doorProbabilityTable.NextItem();
        
        for (var i = 0; i < numDoorsToCreate; i++)
        {
            if (newWallPoints.IsEmpty())
                break;
            
            var door = RNG.RandomIndex(newWallPoints);
            doors.Add(newWallPoints[door]);
            newWallPoints.RemoveAt(door);
        }
        
        walls.AddRange(newWallPoints);
        
        SplitAreaRecursive(splitArea1, walls, doors);
        SplitAreaRecursive(splitArea2, walls, doors);
    }
}