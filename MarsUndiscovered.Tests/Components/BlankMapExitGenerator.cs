using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;

namespace MarsUndiscovered.Tests.Components
{
    public class BlankMapExitGenerator : IMapExitGenerator
    {
        public IMapExitGenerator OriginalMapExitGenerator { get; }

        public BlankMapExitGenerator(IMapExitGenerator originalMapExitGenerator)
        {
            OriginalMapExitGenerator = originalMapExitGenerator;
        }

        public MapExit SpawnMapExit(
            SpawnMapExitParams spawnMapExitParams,
            IGameObjectFactory gameObjectFactory,
            MarsMap map,
            MapExitCollection mapExitCollection
        )
        {
            return null;
        }
    }
}