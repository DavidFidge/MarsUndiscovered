using GoRogue.GameFramework;

namespace MarsUndiscovered.Components.Maps
{
    public interface IItemGenerator
    {
        Item SpawnItem(SpawnItemParams spawnItemParams, Map map, ItemCollection itemCollection);
    }
}