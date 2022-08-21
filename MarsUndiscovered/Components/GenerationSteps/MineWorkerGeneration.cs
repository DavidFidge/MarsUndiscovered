using System.Collections.Generic;
using System.Linq;
using GoRogue.MapGeneration;
using GoRogue.Random;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Components.GenerationSteps
{
    public class MineWorkerGeneration : GenerationStep
    {
        /// <summary>
        /// Optional tag that must be associated with the component used to set wall/floor status of tiles changed by this
        /// algorithm.
        /// </summary>
        public readonly string WallFloorComponentTag;

        public int MinVeinEndpoints { get; set; } = 4;

        public int MaxVeinEndpoints { get; set; } = 10;

        public bool AllowMoreEndpointsToIncreaseMapCoverage { get; set; } = true;
        
        /// <summary>
        /// RNG to use for maze generation.
        /// </summary>
        public IEnhancedRandom RNG { get; set; } = GlobalRandom.DefaultRNG;

        /// <summary>
        /// Creates a new mine worker generation step which simulates veins that have been mined out
        /// </summary>
        /// <param name="name">The name of the generation step.  Defaults to <see cref="MineWorker" />.</param>
        /// <param name="wallFloorComponentTag">
        /// Optional tag that must be associated with the map view component used to store/set
        /// floor/wall status.  Defaults to "WallFloor".
        /// </param>
        public MineWorkerGeneration(string name = null, string wallFloorComponentTag = "WallFloor")
            : base(name)
        {
            WallFloorComponentTag = wallFloorComponentTag;
        }

        /// <inheritdoc/>
        protected override IEnumerator<object> OnPerform(GenerationContext context)
        {
            var wallFloorContext = context.GetFirstOrNew<ISettableGridView<bool>>(
                () => new ArrayView<bool>(context.Width, context.Height),
                WallFloorComponentTag
            );

            var veinPoints = GetVeinPoints();

            foreach (var veinPoint in veinPoints)
            {
                wallFloorContext[veinPoint] = true;
            }
            
            yield return null;

            if (AllowMoreEndpointsToIncreaseMapCoverage)
            {
                var additionalVeinPoints = GetAdditionalVeinPoints(veinPoints);

                foreach (var additionalVeinPoint in additionalVeinPoints)
                {
                    veinPoints.Add(additionalVeinPoint);
                    wallFloorContext[additionalVeinPoint] = true;
                }
            }
            
            yield return null;
        }

        private List<Point> GetAdditionalVeinPoints(List<Point> veinPoints)
        {
            var mapSegments = new List<Rectangle>(4);

            var additionalVeinPoints = new List<Point>();

            var halfMapWidth = MarsMap.MapWidth / 2;
            var halfMapHeight = MarsMap.MapHeight / 2;
            var trailingWidth = MarsMap.MapWidth - halfMapWidth;
            var trailingHeight = MarsMap.MapHeight - halfMapHeight;

            mapSegments.Add(new Rectangle(0, 0, halfMapWidth, halfMapHeight));
            mapSegments.Add(new Rectangle(halfMapWidth, halfMapHeight, trailingWidth, trailingHeight));
            mapSegments.Add(new Rectangle(halfMapWidth, 0, trailingWidth, halfMapHeight));
            mapSegments.Add(new Rectangle(0, halfMapHeight, halfMapWidth, trailingHeight));

            foreach (var segment in mapSegments)
            {
                if (!veinPoints.Any(p => segment.Contains(p)))
                {
                    var newPoint = new Point(RNG.NextInt(segment.MinExtentX, segment.MaxExtentX),
                        RNG.NextInt(segment.MinExtentY, segment.MaxExtentY));
                    additionalVeinPoints.Add(newPoint);
                }
            }

            return additionalVeinPoints;
        }

        private List<Point> GetVeinPoints()
        {
            var numVeinEndpoints = RNG.NextInt(MinVeinEndpoints, MaxVeinEndpoints);

            var veinPoints = new List<Point>(numVeinEndpoints);

            for (var i = 0; i < numVeinEndpoints; i++)
            {
                var newPoint = new Point(RNG.NextInt(MarsMap.MapWidth), RNG.NextInt(MarsMap.MapHeight));
                veinPoints.Add(newPoint);
            }

            return veinPoints;
        }
    }
}
