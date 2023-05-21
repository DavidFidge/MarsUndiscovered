using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using GoRogue.Random;
using MarsUndiscovered.Game.Components.Maps;
using NGenerics.DataStructures.General;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;
using Point = SadRogue.Primitives.Point;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace MarsUndiscovered.Game.Components.GenerationSteps
{
    public class MineWorkerGeneration : GenerationStep
    {
        public int MinVeinRandomEndpoints { get; set; } = 1;
        public int MaxVeinRandomEndpoints { get; set; } = 8;
        public bool AllowMoreEndpointsToIncreaseMapCoverage { get; set; } = true;
        
        public IEnhancedRandom RNG { get; set; } = GlobalRandom.DefaultRNG;

        public MineWorkerGeneration(string name = null)
            : base(name)
        {
        }

        /// <inheritdoc/>
        protected override IEnumerator<object> OnPerform(GenerationContext generationContext)
        {
            var wallFloorContext = generationContext.GetFirstOrNew<ISettableGridView<bool>>(
                () => new ArrayView<bool>(generationContext.Width, generationContext.Height),
                MapGenerator.WallFloorTag
            );
            
            // Get or create/add a tunnel list context component
            var tunnelList = generationContext.GetFirstOrNew(
                () => new ItemList<Area>(),
                MapGenerator.TunnelsTag
            );

            var veinPoints = GetVeinPoints(generationContext);

            foreach (var veinPoint in veinPoints)
            {
                wallFloorContext[veinPoint] = true;
            }
            
            yield return null;

            if (AllowMoreEndpointsToIncreaseMapCoverage)
            {
                var additionalVeinPoints = GetAdditionalVeinPoints(veinPoints, generationContext);

                foreach (var additionalVeinPoint in additionalVeinPoints)
                {
                    veinPoints.Add(additionalVeinPoint);
                    wallFloorContext[additionalVeinPoint] = true;
                }
            }
            
            yield return null;

            var graph = GenerateGraph(veinPoints);
            
            DigTunnel(graph, wallFloorContext, generationContext);

            foreach (var edge in graph.Edges)
            {
                tunnelList.Add((Area)edge.Tag, Name);
            }
            
            yield return null;
        }

        private void DigTunnel(Graph<Point> graph, ISettableGridView<bool> wallFloorContext, GenerationContext generationContext)
        {
            foreach (var edge in graph.Edges)
            {
                var tunnel = new Area();

                var direction = Direction.GetDirection(edge.FromVertex.Data, edge.ToVertex.Data);
                var nextPoint = edge.FromVertex.Data + direction;

                while (direction != Direction.None)
                {
                    var points = nextPoint.PointsOutwardsFrom(1, 1, generationContext.Width - 2, 1, generationContext.Height - 2);

                    foreach (var point in points)
                    {
                        tunnel.Add(point);
                        tunnel.Add(nextPoint);

                        wallFloorContext[point] = true;
                        wallFloorContext[nextPoint] = true;
                    }

                    direction = Direction.GetDirection(nextPoint, edge.ToVertex.Data);
                    nextPoint += direction;
                }

                edge.Tag = tunnel;
            }
        }

        private Graph<Point> GenerateGraph(List<Point> veinPoints)
        {
            var graph = new Graph<Point>(false);

            foreach (var vertex in veinPoints)
            {
                graph.AddVertex(vertex);
            }

            var unconsumedVertices = graph.Vertices.ToList();
            var consumedVertices = new List<Vertex<Point>>(graph.Vertices.Count);

            foreach (var vertex in graph.Vertices)
            {
                unconsumedVertices.Remove(vertex);
                
                if (consumedVertices.Count > 0)
                {
                    var edgeVertex = RNG.RandomElement(consumedVertices);
                    graph.AddEdge(vertex, edgeVertex);
                }
                
                consumedVertices.Add(vertex);
            }
            
            return graph;
        }

        private List<Point> GetAdditionalVeinPoints(List<Point> veinPoints, GenerationContext generationContext)
        {
            var additionalVeinPoints = new List<Point>();

            var mapSegments = BreakIntoSegments(new Rectangle(1, 1, generationContext.Width - 1, generationContext.Height - 1));
            mapSegments = mapSegments.SelectMany(BreakIntoSegments).ToList();

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

        private static List<Rectangle> BreakIntoSegments(Rectangle rectangle)
        {
            var mapSegments = new List<Rectangle>(4);

            var halfWidth = rectangle.Width / 2;
            var halfHeight = rectangle.Height / 2;
            var trailingWidth = rectangle.Width - halfWidth;
            var trailingHeight = rectangle.Height - halfHeight;

            mapSegments.Add(new Rectangle(rectangle.X, rectangle.Y, halfWidth, halfHeight));
            mapSegments.Add(new Rectangle(rectangle.X + halfWidth, rectangle.Y + halfHeight, trailingWidth, trailingHeight));
            mapSegments.Add(new Rectangle(rectangle.X + halfWidth, rectangle.Y, trailingWidth, halfHeight));
            mapSegments.Add(new Rectangle(rectangle.X, rectangle.Y + halfHeight, halfWidth, trailingHeight));
            
            return mapSegments;
        }

        private List<Point> GetVeinPoints(GenerationContext generationContext)
        {
            var numVeinEndpoints = RNG.NextInt(MinVeinRandomEndpoints, MaxVeinRandomEndpoints);

            var veinPoints = new List<Point>(numVeinEndpoints);

            for (var i = 0; i < numVeinEndpoints; i++)
            {
                var newPoint = new Point(RNG.NextInt(1, generationContext.Width - 1), RNG.NextInt(1, generationContext.Height - 1));
                veinPoints.Add(newPoint);
            }

            return veinPoints;
        }
    }
}
