
using MarsUndiscovered.Components.Factories;

namespace MarsUndiscovered.Components.Maps
{
    public interface IMapExitGenerator
    {
        MapExit SpawnMapExit(
            SpawnMapExitParams spawnMapExitParams,
            IGameObjectFactory gameObjectFactory,
            MarsMap map,
            MapExitCollection mapExitCollection
        );
    }
}