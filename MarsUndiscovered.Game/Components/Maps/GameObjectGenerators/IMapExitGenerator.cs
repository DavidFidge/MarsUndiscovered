using MarsUndiscovered.Game.Components.Factories;

namespace MarsUndiscovered.Game.Components.Maps
{
    public interface IMapExitGenerator
    {
        void SpawnMapExit(
            SpawnMapExitParams spawnMapExitParams,
            GameWorld gameWorld
            );
        
        void CreateMapEdgeExits(GameWorld gameWorld,
            MapExitType mapExitType,
            MarsMap map,
            MarsMap linkMap = null);
    }
}
