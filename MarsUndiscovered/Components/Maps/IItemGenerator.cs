using MarsUndiscovered.Components.Factories;

namespace MarsUndiscovered.Components.Maps
{
    public interface IItemGenerator
    {
        Item SpawnItem(SpawnItemParams spawnItemParams, IGameObjectFactory gameObjectFactory, MarsMap map, ItemCollection itemCollection);
    }
}