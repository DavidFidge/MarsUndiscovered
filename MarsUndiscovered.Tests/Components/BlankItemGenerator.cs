using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps;

namespace MarsUndiscovered.Tests.Components
{
    public class BlankItemGenerator : IItemGenerator
    {
        public IItemGenerator OriginalItemGenerator { get; }

        public BlankItemGenerator(IItemGenerator originalItemGenerator)
        {
            OriginalItemGenerator = originalItemGenerator;
        }

        public void SpawnItem(
            SpawnItemParams spawnItemParams,
            IGameObjectFactory gameObjectFactory,
            MapCollection maps,
            ItemCollection itemCollection
        )
        {
        }
    }
}