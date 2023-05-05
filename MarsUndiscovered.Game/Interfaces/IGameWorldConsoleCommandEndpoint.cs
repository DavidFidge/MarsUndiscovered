using MarsUndiscovered.Game.Components;

namespace MarsUndiscovered.Interfaces
{
    public interface IGameWorldConsoleCommandEndpoint
    {
        void SpawnItem(SpawnItemParams spawnItemParams);
        void SpawnMonster(SpawnMonsterParams spawnMonsterParams);
        void SpawnMapExit(SpawnMapExitParams spawnMapExitParams);
    }
}