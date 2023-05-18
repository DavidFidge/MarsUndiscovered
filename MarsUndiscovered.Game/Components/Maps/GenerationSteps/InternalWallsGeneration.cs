using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using MarsUndiscovered.Game.Components.Maps;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.GenerationSteps;

public class InternalWallsGeneration : GenerationStep
{
    public string StepFilterTag { get; set; }
    public string AreasComponentTag { get; set; }

    public InternalWallsGeneration(string? name = null, string? areasComponentTag = "Areas",
        string? stepFilterTag = null)
        : base(name, (typeof(IGridView<bool>), stepFilterTag))
    {
        AreasComponentTag = areasComponentTag; 
        StepFilterTag = stepFilterTag;
    }

    protected override IEnumerator<object> OnPerform(GenerationContext context)
    {
        var wallsFloors = context.GetFirst<GameObjectType>(MapGenerator.WallFloorTypeTag);

        var areas = context
            .GetFirstOrNew(() => new ItemList<Area>(), AreasComponentTag)
            .Where(a => a.Step == StepFilterTag)
            .ToList();
        
    }
}