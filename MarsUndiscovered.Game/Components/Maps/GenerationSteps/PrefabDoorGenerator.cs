using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using GoRogue.Random;
using MarsUndiscovered.Game.Components.Maps;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components.GenerationSteps;

public class PrefabDoorGenerator : GenerationStep
{
    public string PrefabTag { get; set; } = MapGenerator.PrefabTag;
    public string DoorsTag { get; set; } = MapGenerator.DoorsTag;

    public IEnhancedRandom RNG { get; set; } = GlobalRandom.DefaultRNG;

    public PrefabDoorGenerator(string name = null)
        : base(name)
    {
    }
    
    protected override IEnumerator<object> OnPerform(GenerationContext generationContext)
    {
        var prefabContext = generationContext.GetFirstOrNew<ItemList<PrefabInstance>>(
            () => new ItemList<PrefabInstance>(),
            PrefabTag
        );
        
        var doors = generationContext.GetFirstOrNew(() => new ItemList<GameObjectTypePosition<DoorType>>(), DoorsTag);

        // If performance becomes a problem then prefab characters could be keyed
        foreach (var prefab in prefabContext.Items)
        {
            foreach (var point in prefab.Area)
            {
                var isDoor = prefab.GetPrefabCharAt(point) == Constants.DoorPrefab;

                if (isDoor)
                    doors.Add(new GameObjectTypePosition<DoorType>(DoorType.DefaultDoor, point), Name);
            }
        }
        
        yield return null;
    }
}