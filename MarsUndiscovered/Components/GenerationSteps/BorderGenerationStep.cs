using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.MapGeneration;
using GoRogue.MapGeneration.Steps;

using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components.GenerationSteps
{
    /// <summary>
    /// Sets map border to FillValue. Border defines the size of the map border.
    /// </summary>
    public class BorderGenerationStep : GenerationStep
    {
        public bool FillValue { get; }

        /// <summary>
        /// Optional tag that must be associated with the grid view that random values are set to.
        /// </summary>
        public readonly string GridViewComponentTag;

        /// <summary>
        /// Border size
        /// </summary>
        public int Border = 1;

        /// <summary>
        /// Creates a new step for applying random values to a map view.
        /// </summary>
        /// <param name="name">The name of the generation step.  Defaults to <see cref="RandomViewFill" />.</param>
        /// <param name="gridViewComponentTag">
        /// Optional tag that must be associated with the grid view that random values are set to.  Defaults to
        /// "WallFloor".
        /// </param>
        /// <param name="fillValue">Value used for the fill, either true or false</param>
        public BorderGenerationStep(string name = null, string gridViewComponentTag = "WallFloor", bool fillValue = true)
            : base(name)
        {
            FillValue = fillValue;
            GridViewComponentTag = gridViewComponentTag;
        }

        /// <inheritdoc/>
        protected override IEnumerator<object> OnPerform(GenerationContext context)
        {
            // Get or create/add a grid view context component to fill
            var gridViewContext = context.GetFirstOrNew<ISettableGridView<bool>>(
                () => new ArrayView<bool>(context.Width, context.Height),
                GridViewComponentTag);

            foreach (var position in gridViewContext.Bounds().PerimeterBorder(Border))
            {
                gridViewContext[position] = FillValue;
            }

            yield break;
        }
    }
}
