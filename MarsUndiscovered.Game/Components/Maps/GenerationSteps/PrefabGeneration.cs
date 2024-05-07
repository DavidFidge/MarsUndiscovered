using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
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
        private readonly IPrefabProvider _prefabProvider;
        private List<Prefab> _prefabs => _prefabProvider.Prefabs;
        
        public IEnhancedRandom RNG { get; set; } = GlobalRandom.DefaultRNG;
        
        public PrefabGeneration(IPrefabProvider prefabProvider, string name = null)
            : base(name)
        {
            _prefabProvider = prefabProvider;
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
            
            var prefabContext = generationContext.GetFirstOrNew<ItemList<PrefabInstance>>(
                () => new ItemList<PrefabInstance>(),
                MapGenerator.PrefabTag
            );
            
            foreach (var item in prefabInstances)
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

                var randomPrefab = RNG.RandomElement(_prefabs);

                var maxPotentialLocation = wallFloorContext.Bounds().MaxExtent - randomPrefab.Bounds.MaxExtent;
                var potentialBounds = new Rectangle(Point.Zero, maxPotentialLocation);
                
                // find random point
                var failedNewPrefabPlacementCount = 0;
                
                while (failedNewPrefabPlacementCount < 5)
                {
                    var randomPoint = RNG.RandomPosition(potentialBounds);
                    var randomDirection = RNG.RandomElement(AdjacencyRule.Cardinals.DirectionsOfNeighborsCache);
                    var isMirrored = RNG.NextBool();
                    
                    var newPrefab = new PrefabInstance(randomPrefab, randomPoint, randomDirection, isMirrored);
                    
                    var canPlaceAvailableArea = availablePlacementAreas.FirstOrDefault(a => a.Contains(newPrefab.Area));

                    if (canPlaceAvailableArea != null)
                    {
                        prefabInstances.Add(newPrefab);
                        prefabPlaced = true;

                        foreach (var point in newPrefab.Area)
                        {
                            var isFloor = newPrefab.GetPrefabCharAt(point) == Constants.FloorPrefab;
                        
                            // if a floor, change the wall/floor context
                            wallFloorContext[point] = isFloor;
                            
                            // Any walls (#) in prefabs can be used by other prefabs.
                            // (If X then it is a wall that is reserved)
                            var isUnused = newPrefab.GetPrefabCharAt(point) == Constants.WallPrefab;

                            freeSpaceForCreatingPrefabs[point] = isUnused;

                            if (isFloor)
                            {
                                // prefabs can overlap on its walls but not floors. Remove floor points
                                // from future prefab placements.
                                canPlaceAvailableArea.Remove(point);
                            }
                        }
                        
                        break;
                    }

                    failedNewPrefabPlacementCount++;
                }

                if (!prefabPlaced)
                    failedPlacementCount++;
            }
        }
    }
}
