using GoRogue.MapGeneration;
using GoRogue.Random;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Game.Extensions;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
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

            var negatedWallFloorContext = wallFloorContext.ToArrayView(c => !c);
            
            var availablePlacementAreas = MapAreaFinder.MapAreasFor(negatedWallFloorContext,AdjacencyRule.EightWay);

            var failedPlacementCount = 0;
            
            while (failedPlacementCount < 10)
            {
                var prefabPlaced = false;

                var randomPrefab = RNG.RandomElement(Prefabs);
                
                var maxPotentialLocation = wallFloorContext.Bounds().Size - randomPrefab.Bounds.Size - 1;
                var potentialBounds = new Rectangle(0, 0 , maxPotentialLocation.X, maxPotentialLocation.Y);
                
                // find random point
                var failedNewPrefabPlacementCount = 0;

                while (failedPlacementCount < 5)
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
                        
                            wallFloorContext[point] = isFloor;
                        }

                        yield return null;
                    }

                    failedNewPrefabPlacementCount++;
                }

                if (!prefabPlaced)
                    failedPlacementCount++;
            }

            foreach (var prefab in prefabInstances)
            {
                var sourceConnectorPoint = prefab.GetRandomConnectorPoint(RNG);

                var connectingPrefab = RNG.RandomElement(prefabInstances.Where(p => p != prefab).ToList());

                var destinationConnectorPoint = prefab.GetRandomConnectorPoint(RNG);

                // It is okay if the tunnel goes across existing tunnels, no need to update negatedWallFloorContext for each tunnel.
                var tunnelCreator = new AStarTunnelCreator(negatedWallFloorContext, Distance.Euclidean);

                tunnelCreator.CreateTunnel(wallFloorContext, sourceConnectorPoint, destinationConnectorPoint);

                yield return null;
            }
        }
    }
}
