using GoRogue.GameFramework;

using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;

namespace MarsUndiscovered.Tests.Components
{
    public class BlankItemGenerator : IItemGenerator
    {
        public IItemGenerator OriginalItemGenerator { get; }

        public BlankItemGenerator(IItemGenerator originalItemGenerator)
        {
            OriginalItemGenerator = originalItemGenerator;
        }

        public Item SpawnItem(
            SpawnItemParams spawnItemParams,
            IGameObjectFactory gameObjectFactory,
            Map map,
            ItemCollection itemCollection
        )
        {
            return null;
        }
    }
}