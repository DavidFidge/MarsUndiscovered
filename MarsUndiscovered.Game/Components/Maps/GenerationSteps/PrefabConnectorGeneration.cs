using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using GoRogue.Random;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Game.Extensions;
using NGenerics.DataStructures.General;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Collections;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components.GenerationSteps
{
    public class PrefabConnectorGeneration : GenerationStep
    {
        public IEnhancedRandom RNG { get; set; } = GlobalRandom.DefaultRNG;

        public PrefabConnectorGeneration(string name = null)
            : base(name)
        {
        }
        
        protected override IEnumerator<object> OnPerform(GenerationContext generationContext)
        {
            var wallFloorContext = generationContext.GetFirstOrNew<ISettableGridView<bool>>(
                () => new ArrayView<bool>(generationContext.Width, generationContext.Height),
                MapGenerator.WallFloorTag
            );
            
            var prefabContext = generationContext.GetFirstOrNew<ItemList<PrefabInstance>>(
                () => new ItemList<PrefabInstance>(),
                MapGenerator.PrefabTag
            );

            var prefabInstances = prefabContext.Items.ToList();
            
            // Floors are True in wallFloorContext.  We need to build an area for the walls as the walls are the free areas that the prefabs can be placed in. So this can be done by negating so that walls are True.
            var freeSpaceForConnectingPrefabs = wallFloorContext.ToArrayView(c => !c);

            foreach (var prefab in prefabInstances)
            {
                foreach (var point in prefab.Area)
                {
                    // Need to exclude the 'Connector' walls from being valid points to cut through
                    // as well as any other types that should not be tunneled through
                    var cannotTunnelThrough = prefab.GetPrefabCharAt(point) == Constants.ConnectorPrefab || 
                                              prefab.GetPrefabCharAt(point) == Constants.WallDoNotTunnel;

                    if (cannotTunnelThrough)
                        freeSpaceForConnectingPrefabs[point] = false;
                }
            }
            
            var prefabDistanceGraph = GetPrefabDistanceGraph(prefabInstances);

            var prefabTunnelGraph = new Graph<PrefabInstance>(false);

            foreach (var prefab in prefabInstances)
            {
                var prefabDistanceGraphVertex = prefabDistanceGraph.GetVertex(prefab);
                var prefabTunnelGraphVertex = prefabTunnelGraph.GetVertex(prefab);

                var toVertexesToExclude = prefabTunnelGraphVertex?.NeighboringVertices()
                    .Select(e => e.Data)
                    .ToList() ?? new List<PrefabInstance>();

                // prefabs further out will have a weight of 1, the closest prefab will have a weight of prefabInstances.Count
                var prefabDistanceWeights = prefabDistanceGraphVertex.NeighboringVertices()
                    .Where(e => !toVertexesToExclude.Contains(e.Data))
                    .OrderByDescending(e => e.Weight)
                    .Select((e, index) => (e.Data, (double)index * 2 + 1))
                    .ToList();
                
                var sourceConnectorPoint = prefab.GetRandomConnectorPoint(RNG);
                
                var prefabConnectionTries = 0;
                
                var probabilityTable = new ProbabilityTable<PrefabInstance>(prefabDistanceWeights)
                {
                    Random = RNG
                };
                
                while (probabilityTable.Count > 0 && prefabConnectionTries < 10)
                {
                    var connectingPrefab = probabilityTable.NextItem();

                    var destinationConnectorPoint = connectingPrefab.GetRandomConnectorPoint(RNG);
                    
                    // Allow the source and destination points to be cut through
                    freeSpaceForConnectingPrefabs[sourceConnectorPoint] = true;
                    freeSpaceForConnectingPrefabs[destinationConnectorPoint] = true;
                    
                    // Create tunnel. At the moment we aren't updating freeSpaceForCreatingPrefabs, at this stage it is okay for tunnels to cross each other.
                    var tunnelCreator = new AStarTunnelCreator(freeSpaceForConnectingPrefabs, Distance.Euclidean, true);

                    var result = tunnelCreator.CreateTunnel(wallFloorContext, sourceConnectorPoint,
                        destinationConnectorPoint);

                    if (result.Any())
                    {
                        var edge = prefabTunnelGraph.AddEdge(prefabTunnelGraph.GetOrAddVertex(prefab), prefabTunnelGraph.GetOrAddVertex(connectingPrefab));
                        edge.Tag = result;
                        edge.Weight = result.Count;
                        
                        yield return null;
                        break;
                    }

                    // could not cut through, need to disallow cutting through connection points
                    freeSpaceForConnectingPrefabs[sourceConnectorPoint] = false;
                    freeSpaceForConnectingPrefabs[destinationConnectorPoint] = false;

                    prefabConnectionTries++;
                }
            }
            
            var unconnectedPrefabs = prefabTunnelGraph.Vertices
                .Where(v => v.Degree == 0)
                .Select(v => v.Data)
                .ToList();
            
            // Convert prefab back to wall
            foreach (var prefab in unconnectedPrefabs)
            {
                foreach (var point in prefab.Area.ToList())
                {
                    wallFloorContext[point] = false;
                }
            }
        }
        
        private static Graph<PrefabInstance> GetPrefabDistanceGraph(List<PrefabInstance> prefabInstances)
        {
            var prefabDistanceGraph = new Graph<PrefabInstance>(false);

            if (prefabInstances.Count == 0)
                return new Graph<PrefabInstance>(false);
            
            else if (prefabInstances.Count == 1)
            {
                prefabDistanceGraph.AddVertex(prefabInstances[0]);
                return prefabDistanceGraph;
            }
            
            for (var inner = 0; inner < prefabInstances.Count - 1; inner++)
            {
                for (var outer = inner + 1; outer < prefabInstances.Count; outer++)
                {
                    var edge = prefabDistanceGraph.AddEdge(prefabDistanceGraph.GetOrAddVertex(prefabInstances[inner]), prefabDistanceGraph.GetOrAddVertex(prefabInstances[outer]));
                    edge.Weight = Distance.Chebyshev.Calculate(prefabInstances[inner].Location, prefabInstances[outer].Location);
                }
            }

            return prefabDistanceGraph;
        }
    }
}
