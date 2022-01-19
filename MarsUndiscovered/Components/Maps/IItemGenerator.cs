using GoRogue.GameFramework;

using MarsUndiscovered.Components.Factories;

namespace MarsUndiscovered.Components.Maps
{
    public interface IItemGenerator
    {
        Item SpawnItem(SpawnItemParams spawnItemParams, IGameObjectFactory gameObjectFactory, Map map, ItemCollection itemCollection);
    }
}