using MarsUndiscovered.Game.Components.Factories;

namespace MarsUndiscovered.Game.Components.Maps
{
    public interface IItemGenerator
    {
        Item SpawnItem(SpawnItemParams spawnItemParams, IGameObjectFactory gameObjectFactory, MapCollection maps, ItemCollection itemCollection);
    }
}