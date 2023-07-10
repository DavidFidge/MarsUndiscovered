using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.MapGeneration;
using MarsUndiscovered.Game.Components.Maps;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.GenerationSteps
{
    /// <summary>
    /// Sets map border to FillValue. Border defines the size of the map border.
    /// </summary>
    public class BorderGenerationStep : GenerationStep
    {
        public bool FillValue { get; }
        public int Border { get; set; } = 1;

        public BorderGenerationStep(string name = null, bool fillValue = true)
            : base(name)
        {
            FillValue = fillValue;
        }

        /// <inheritdoc/>
        protected override IEnumerator<object> OnPerform(GenerationContext generationContext)
        {
            // Get or create/add a grid view context component to fill
            var gridViewContext = generationContext.GetFirstOrNew<ISettableGridView<bool>>(
                () => new ArrayView<bool>(generationContext.Width, generationContext.Height),
                MapGenerator.WallFloorTag);

            foreach (var position in gridViewContext.Bounds().PerimeterBorder(Border))
            {
                gridViewContext[position] = FillValue;
            }

            yield return null;
        }
    }
}
