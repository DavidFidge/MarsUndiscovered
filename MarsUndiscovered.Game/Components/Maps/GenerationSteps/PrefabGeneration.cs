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
    public class PrefabInstance
    {
        public Prefab Prefab { get; set; }
        public Point Location { get; set; }
    }
    
    public class Prefab
    {
        public string[] PrefabText { get; set; }
        public Rectangle Bounds => new Rectangle(0, 0, PrefabText[0].Length, PrefabText.Length);
    }
    
    public class PrefabGeneration : GenerationStep
    {
        public List<Prefab> Prefabs { get; set; } = new();
        
        public IEnhancedRandom RNG { get; set; } = GlobalRandom.DefaultRNG;

        public PrefabGeneration(string name = null)
            : base(name)
        {
            var prefab1 = new Prefab
            {
                PrefabText = new[]
                {
                    "#########",
                    "#.......#",
                    "#.......#",
                    "#.......#",
                    "#.......#",
                    "#.......#",
                    "#.......#",
                    "#.......#",
                    "#########"
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
                Location = new Point(20, 20)
            };

            prefabInstances.Add(prefabInstance1);
            prefabInstances.Add(prefabInstance2);

            foreach (var prefabInstance in prefabInstances)
            {
                var prefab = prefabInstance.Prefab;
                var location = prefabInstance.Location;
                
                for (var y = 0; y < prefab.Bounds.Height; y++)
                {
                    for (var x = 0; x < prefab.Bounds.Width; x++)
                    {
                        var prefabChar = prefab.PrefabText[y][x];
                        
                        var isWall = prefabChar == '#';
                        
                        wallFloorContext[location.X + x, location.Y + y] = isWall;
                    }
                }
            }
            
            yield return null;
        }
    }
}
