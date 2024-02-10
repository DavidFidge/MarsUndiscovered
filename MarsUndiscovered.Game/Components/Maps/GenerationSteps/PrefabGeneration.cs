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
    public class PrefabGeneration : GenerationStep
    {
        public List<Prefab> Prefabs { get; set; } = new();
        
        public IEnhancedRandom RNG { get; set; } = GlobalRandom.DefaultRNG;

        public PrefabGeneration(string name = null)
            : base(name)
        {
            // C = potential connection point
            // # = wall
            // . = floor

            var prefabText1 = new[]
            {
                "#CCCCCCC#",
                "C.......C",
                "C.......C",
                "C.......C",
                "C.......C",
                "C.......C",
                "C.......C",
                "C.......C",
                "#CCCCCCC#"
            };

            var prefab1 = new Prefab(prefabText1);
            Prefabs.Add(prefab1);
        }
        
        protected override IEnumerator<object> OnPerform(GenerationContext generationContext)
        {
            var wallFloorContext = generationContext.GetFirstOrNew<ISettableGridView<bool>>(
                () => new ArrayView<bool>(generationContext.Width, generationContext.Height),
                MapGenerator.WallFloorTag
            );

            var prefabInstances = new List<PrefabInstance>();
            
            // Floors are True in wallFloorContext.  We need to build an area for the walls as the walls are the free areas that the prefabs can be placed in. So this can be done by negating so that walls are True.
            var freeSpaceForCreatingPrefabs = wallFloorContext.ToArrayView(c => !c);
            
            CreatePrefabs(freeSpaceForCreatingPrefabs, wallFloorContext, prefabInstances);

            yield return null;
            
            // TODO the tunnel connecting can go in a separate generator 
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

                    // Create tunnel. At the moment we aren't updating freeSpaceForCreatingPrefabs, at this stage it is okay for tunnels to cross each other.
                    var tunnelCreator = new AStarTunnelCreator(freeSpaceForCreatingPrefabs, Distance.Euclidean, true);

                    var temp1 = GameObjectWriter.WriteGridView(wallFloorContext);

                    var result = tunnelCreator.CreateTunnel(wallFloorContext, sourceConnectorPoint,
                        destinationConnectorPoint);

                    if (result.Any())
                    {
                        var edge = prefabTunnelGraph.AddEdge(prefabTunnelGraph.GetOrAddVertex(prefab), prefabTunnelGraph.GetOrAddVertex(connectingPrefab));
                        edge.Tag = result;
                        edge.Weight = result.Count;
                        break;
                    }

                    prefabConnectionTries++;
                }

                var temp2 = GameObjectWriter.WriteGridView(wallFloorContext);

                yield return null;
            }
            
            var temp3 = GameObjectWriter.WriteGridView(wallFloorContext);
            
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
            
            var temp4 = GameObjectWriter.WriteGridView(wallFloorContext);
            
            var connectedPrefabs = prefabTunnelGraph.Vertices
                .Where(v => v.Degree > 0)
                .Select(v => v.Data)
                .ToList();
            
            var prefabContext = generationContext.GetFirstOrNew<ItemList<PrefabInstance>>(
                () => new ItemList<PrefabInstance>(),
                MapGenerator.PrefabTag
            );
            
            foreach (var item in connectedPrefabs)
            {
                prefabContext.Add(item, nameof(PrefabGeneration));
            }
        }

        private void CreatePrefabs(ArrayView<bool> freeSpaceForCreatingPrefabs, ISettableGridView<bool> wallFloorContext,
            List<PrefabInstance> prefabInstances)
        {
            // Build an Area object for placing the prefabs. Areas are easier to work with since it has Intersect-like methods.
            var availablePlacementAreas = MapAreaFinder
                .MapAreasFor(
                    freeSpaceForCreatingPrefabs, 
                    AdjacencyRule.EightWay)
                .ToList();

            var failedPlacementCount = 0;
            
            while (failedPlacementCount < 10)
            {
                var prefabPlaced = false;

                var randomPrefab = RNG.RandomElement(Prefabs);
                
                var maxPotentialLocation = wallFloorContext.Bounds().Size - randomPrefab.Bounds.Size - 1;
                var potentialBounds = new Rectangle(0, 0 , maxPotentialLocation.X, maxPotentialLocation.Y);
                
                // find random point
                var failedNewPrefabPlacementCount = 0;

                while (failedNewPrefabPlacementCount < 5)
                {
                    var randomPoint = RNG.RandomPosition(potentialBounds);

                    var newPrefab = new PrefabInstance(randomPrefab, randomPoint);

                    var canPlace = availablePlacementAreas.FirstOrDefault(a => a.Contains(newPrefab.Area));

                    if (canPlace != null)
                    {
                        prefabInstances.Add(newPrefab);
                        canPlace.Remove(newPrefab.Area);
                        prefabPlaced = true;

                        foreach (var point in newPrefab.Area.ToList())
                        {
                            var isFloor = newPrefab.GetPrefabCharAt(point) == Constants.FloorPrefab;
                        
                            // if a floor, change the wall/floor context
                            wallFloorContext[point] = isFloor;
                            
                            var isUnused = newPrefab.GetPrefabCharAt(point) == Constants.UnusedPrefab;

                            freeSpaceForCreatingPrefabs[point] = isUnused;
                        }
                        
                        break;
                    }

                    failedNewPrefabPlacementCount++;
                }

                if (!prefabPlaced)
                    failedPlacementCount++;
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
