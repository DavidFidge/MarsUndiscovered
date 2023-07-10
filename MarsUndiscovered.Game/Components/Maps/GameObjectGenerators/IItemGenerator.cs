using MarsUndiscovered.Game.Components.Factories;

namespace MarsUndiscovered.Game.Components.Maps
{
    public interface IItemGenerator
    {
        void SpawnItem(SpawnItemParams spawnItemParams, IGameObjectFactory gameObjectFactory, MapCollection maps, ItemCollection itemCollection);
    }
}