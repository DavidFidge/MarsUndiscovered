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
            var freeSpaceForAStarConnectingPrefabs = wallFloorContext.ToArrayView(c => !c);

            foreach (var prefab in prefabInstances)
            {
                foreach (var point in prefab.Area)
                {
                    // Need to exclude the 'Connector' walls from being valid points to cut through
                    // as well as any other types that should not be tunneled through
                    var cannotTunnelThrough = prefab.GetPrefabCharAt(point) == Constants.ConnectorPrefab || 
                                              prefab.GetPrefabCharAt(point) == Constants.WallDoNotTunnel;

                    if (cannotTunnelThrough)
                        freeSpaceForAStarConnectingPrefabs[point] = false;
                }
            }
            
            var prefabDistanceGraph = GetPrefabDistanceGraph(prefabInstances);

            var prefabConnections =
                prefabDistanceGraph.Vertices.ToDictionary(
                    k => k.Data,
                    v => new List<PrefabInstance>());
            
            foreach (var prefab in prefabInstances)
            {
                var prefabDistanceGraphVertex = prefabDistanceGraph.GetVertex(prefab);
                var toVertexesToExclude = prefabConnections[prefab];

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
                    
                    var result = TryCreateTunnel(
                        freeSpaceForAStarConnectingPrefabs,
                        wallFloorContext, sourceConnectorPoint,
                        destinationConnectorPoint,
                        prefabConnections,
                        prefab,
                        connectingPrefab);

                    if (result.Any())
                    {
                        // Interesting feature found - not breaking here creates a map that looks like it has 
                        // been bored through.
                        break;
                    }

                    prefabConnectionTries++;
                }
            }
            
            yield return null;

            // connect prefabs where their walls intersect
            var overlappingPairs = new List<(PrefabInstance, PrefabInstance)>();

            for (var i = 0; i < prefabInstances.Count - 1; i++)
            {
                for (var j = i + 1; j < prefabInstances.Count; j++)
                {
                    var prefab1 = prefabInstances[i];
                    var prefab2 = prefabInstances[j];

                    if (prefab1.Area.Intersects(prefab2.Area))
                    {
                        overlappingPairs.Add((prefab1, prefab2));
                    }
                }
            }
            
            foreach (var (prefab1, prefab2) in overlappingPairs)
            {
                var intersection = Area.GetIntersection(prefab1.Area, prefab2.Area);

                var intersectionConnectionPoints = intersection
                    .Where(p => prefab1.GetPrefabCharAt(p) == Constants.ConnectorPrefab && prefab2.GetPrefabCharAt(p) == Constants.ConnectorPrefab)
                    .ToList();
                
                if (!intersectionConnectionPoints.Any())
                    continue;

                var connectionPoint = RNG.RandomElement(intersectionConnectionPoints);
         
                var result = TryCreateTunnel(
                    freeSpaceForAStarConnectingPrefabs,
                    wallFloorContext,
                    connectionPoint,
                    connectionPoint,
                    prefabConnections,
                    prefab1,
                    prefab2);
            }

            yield return null;

            // remove unconnected prefabs
            var unconnectedPrefabs = prefabConnections
                .Where(kvp => kvp.Value.Count == 0)
                .Select(kvp => kvp.Key)
                .ToList();
            
            // Convert prefab back to wall
            foreach (var prefab in unconnectedPrefabs)
            {
                foreach (var point in prefab.Area.ToList())
                {
                    wallFloorContext[point] = false;
                }
            }
            
            yield return null;
        }

        private static Area TryCreateTunnel(
            ArrayView<bool> freeSpaceForAStarConnectingPrefabs,
            ISettableGridView<bool> wallFloorContext,
            Point sourceConnectorPoint,
            Point destinationConnectorPoint,
            Dictionary<PrefabInstance, List<PrefabInstance>> prefabConnections,
            PrefabInstance prefab,
            PrefabInstance connectingPrefab)
        {
            // Create tunnel. At the moment we aren't updating freeSpaceForCreatingPrefabs, at this stage it is okay for tunnels to cross each other.
            // Allow the source and destination points to be cut through
            freeSpaceForAStarConnectingPrefabs[sourceConnectorPoint] = true;
            freeSpaceForAStarConnectingPrefabs[destinationConnectorPoint] = true;
            
            var tunnelCreator = new AStarTunnelCreator(freeSpaceForAStarConnectingPrefabs, Distance.Euclidean, true);

            var result = tunnelCreator.CreateTunnel(wallFloorContext, sourceConnectorPoint,
                destinationConnectorPoint);

            if (result.Any())
            {
                prefabConnections[prefab].Add(connectingPrefab);
                prefabConnections[connectingPrefab].Add(prefab);
            }
            else
            {
                // could not cut through, need to disallow cutting through connection points
                freeSpaceForAStarConnectingPrefabs[sourceConnectorPoint] = false;
                freeSpaceForAStarConnectingPrefabs[destinationConnectorPoint] = false;
            }

            return result;
        }

        private static Graph<PrefabInstance> GetPrefabDistanceGraph(List<PrefabInstance> prefabInstances)
        {
            var prefabDistanceGraph = new Graph<PrefabInstance>(false);

            if (prefabInstances.Count == 0)
                return new Graph<PrefabInstance>(false);
            
            if (prefabInstances.Count == 1)
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
