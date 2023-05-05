using MarsUndiscovered.Game.Components.Factories;

namespace MarsUndiscovered.Game.Components.Maps
{
    public interface IMapExitGenerator
    {
        MapExit SpawnMapExit(
            SpawnMapExitParams spawnMapExitParams,
            IGameObjectFactory gameObjectFactory,
            MapCollection maps,
            MapExitCollection mapExitCollection
        );
    }
}
