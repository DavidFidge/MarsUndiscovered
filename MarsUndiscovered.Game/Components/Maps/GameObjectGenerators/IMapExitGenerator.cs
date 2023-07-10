using MarsUndiscovered.Game.Components.Factories;

namespace MarsUndiscovered.Game.Components.Maps
{
    public interface IMapExitGenerator
    {
        void SpawnMapExit(
            SpawnMapExitParams spawnMapExitParams,
            IGameObjectFactory gameObjectFactory,
            MapCollection maps,
            MapExitCollection mapExitCollection
        );
    }
}
