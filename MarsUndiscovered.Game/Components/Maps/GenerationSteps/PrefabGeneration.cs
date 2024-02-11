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
    }
}
