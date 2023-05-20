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

        foreach (var area in areas.Select(a => a.Item))
        {
            var splitVertical = area.Bounds.Width > area.Bounds.Height;
            var splitLength = splitVertical ? area.Bounds.Width : area.Bounds.Height;
            
            if (splitLength <= _splitFactor)
                continue;
            
            if (splitVertical && area.Bounds.Width <= 5)
                splitVertical = false;

            if (splitVertical)
            {
                var splitPoint = RNG.NextInt(area.Bounds.MinExtentX + 1, area.Bounds.MaxExtentX - 1);
                var area1 = 
                
            }
        }
    }
}