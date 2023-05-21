using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using GoRogue.Random;
using MarsUndiscovered.Game.Components.Maps;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components.GenerationSteps;

public class InternalWallsGeneration : GenerationStep
{
    private readonly int _splitFactor;
    public string StepFilterTag { get; set; }
    public string AreasComponentTag { get; set; }
    public string AreaWallsDoorsComponentTag { get; set; }

    public IEnhancedRandom RNG { get; set; } = GlobalRandom.DefaultRNG;

    public InternalWallsGeneration(string? name = null, int splitFactor = 5, string? areasComponentTag = "Areas",
        string? stepFilterTag = null)
        : base(name, (typeof(IGridView<bool>), stepFilterTag))
    {
        _splitFactor = splitFactor;
        AreasComponentTag = areasComponentTag; 
        StepFilterTag = stepFilterTag;
    }

    protected override IEnumerator<object> OnPerform(GenerationContext context)
    {
        var wallsFloors = context.GetFirst<GameObjectType>(MapGenerator.WallFloorTypeTag);

        var areas = context
            .GetFirstOrNew(() => new ItemList<Area>(), AreasComponentTag)
            .Where(a => String.IsNullOrEmpty(StepFilterTag) || a.Step == StepFilterTag)
            .ToList();

        var areaWallsDoors = context.GetFirstOrNew(() => new ItemList<AreaWallsDoors>(), AreaWallsDoorsComponentTag);

        foreach (var area in areas.Select(a => a.Item))
        {
            var doors = new List<Point>();
            var walls = new List<Point>();

            SplitAreaRecursive(area, walls, doors);

            areaWallsDoors.Add(
                new AreaWallsDoors
                {
                    Area = area,
                    Walls = walls,
                    Doors = doors
                },
                Name
                );
        }

        yield return null;
    }

    private void SplitAreaRecursive(Area area, List<Point> walls, List<Point> doors)
    {
        var splitVertical = area.Bounds.Width > area.Bounds.Height;
        var splitLength = splitVertical ? area.Bounds.Width : area.Bounds.Height;

        if (splitLength <= _splitFactor)
            return;

        List<Point> newWallPoints;
        Area splitArea1;
        Area splitArea2;

        if (splitVertical)
        {
            var splitPoint = RNG.NextInt(area.Bounds.MinExtentX + 1, area.Bounds.MaxExtentX - 1);
            newWallPoints = area.Where(p => p.X == splitPoint).ToList();
            splitArea1 = new Area(area.Where(p => p.X < splitPoint));
            splitArea2 = new Area(area.Where(p => p.X > splitPoint));
        }
        else
        {
            var splitPoint = RNG.NextInt(area.Bounds.MinExtentY + 1, area.Bounds.MaxExtentY - 1);
            newWallPoints = area.Where(p => p.Y == splitPoint).ToList();
            splitArea1 = new Area(area.Where(p => p.Y < splitPoint));
            splitArea2 = new Area(area.Where(p => p.Y > splitPoint));
        }
        
        var door = RNG.RandomIndex(newWallPoints);
        doors.Add(newWallPoints[door]);
        newWallPoints.RemoveAt(door);
        
        walls.AddRange(newWallPoints);
        
        SplitAreaRecursive(splitArea1, walls, doors);
        SplitAreaRecursive(splitArea2, walls, doors);
    }
}