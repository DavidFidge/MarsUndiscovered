using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.GenerationSteps;

/// <summary>
/// Finds the distinct areas in the boolean grid view specified, and adds them to the item list with the tag
/// specified.
/// </summary>
public class GenericAreaFinder<T> : GenerationStep
{
    /// <summary>
    /// Optional tag that must be associated with the grid view used to find areas.
    /// </summary>
    public readonly string GridViewComponentTag;

    private readonly Func<T, bool> _resolver;

    /// <summary>
    /// Optional tag that must be associated with the component used to store areas found by this algorithm.
    /// </summary>
    public readonly string AreasComponentTag;

    /// <summary>
    /// The adjacency method to use for determining whether two locations are in the same area.
    /// </summary>
    public AdjacencyRule AdjacencyMethod = AdjacencyRule.Cardinals;
    
    public GenericAreaFinder(Func<T, bool> resolver, string name = null, string gridViewComponentTag = "WallFloor",
                      string areasComponentTag = "Areas")
        : base(name, (typeof(IGridView<T>), gridViewComponentTag))
    {
        _resolver = resolver;
        AreasComponentTag = areasComponentTag;
        GridViewComponentTag = gridViewComponentTag;
    }

    /// <inheritdoc/>
    protected override IEnumerator<object> OnPerform(GenerationContext context)
    {
        // Get/create required components
        var arrayView = context.GetFirst<IGridView<T>>(GridViewComponentTag); // Known to succeed because required
        
        var gridView = new LambdaGridView<bool>(arrayView.Width, arrayView.Height, p=> _resolver(arrayView[p.X, p.Y]));
        
        var areas = context.GetFirstOrNew(() => new ItemList<Area>(), AreasComponentTag);

        // Use MapAreaFinder to find unique areas and record them in the correct component
        areas.AddRange(MapAreaFinder.MapAreasFor(gridView, AdjacencyMethod), Name);

        yield break;
    }
}
