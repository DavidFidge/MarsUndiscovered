using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using GoRogue.MapGeneration.TunnelCreators;
using GoRogue.Random;
using MarsUndiscovered.Game.Components.Maps;
using NGenerics.DataStructures.General;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;
using Point = SadRogue.Primitives.Point;

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
            var prefab1 = new Prefab
            {
                PrefabText = new[]
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
                }
            };
            
            Prefabs.Add(prefab1);
        }
        
        protected override IEnumerator<object> OnPerform(GenerationContext generationContext)
        {
            var wallFloorContext = generationContext.GetFirstOrNew<ISettableGridView<bool>>(
                () => new ArrayView<bool>(generationContext.Width, generationContext.Height),
                MapGenerator.WallFloorTag
            );

            var prefabInstances = new List<PrefabInstance>();
            
            var prefabInstance1 = new PrefabInstance
            {
                Prefab = RNG.RandomElement(Prefabs),
                Location = new Point(0, 0)
            };
            
            var prefabInstance2 = new PrefabInstance
            {
                Prefab = RNG.RandomElement(Prefabs),
                Location = new Point(10, 1)
            };

            prefabInstances.Add(prefabInstance1);
            prefabInstances.Add(prefabInstance2);

            PrefabInstance lastPrefabInstance = null;
            
            foreach (var prefabInstance in prefabInstances)
            {
                var prefab = prefabInstance.Prefab;
                var location = prefabInstance.Location;
                
                for (var y = 0; y < prefab.Bounds.Height; y++)
                {
                    for (var x = 0; x < prefab.Bounds.Width; x++)
                    {
                        var prefabChar = prefab.PrefabText[y][x];
                        
                        var isFloor = prefabChar != '#';
                        
                        wallFloorContext[location.X + x, location.Y + y] = isFloor;
                    }
                }

                if (lastPrefabInstance != null)
                {
                    var lineCreator = new DirectLineTunnelCreator(AdjacencyRule.Cardinals);
                    //TODO tunnelling
                }

                lastPrefabInstance = prefabInstance;
            }
            
            yield return null;
        }
    }
}
