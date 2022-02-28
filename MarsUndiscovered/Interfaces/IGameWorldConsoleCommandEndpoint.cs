using MarsUndiscovered.Components;

namespace MarsUndiscovered.Interfaces
{
    public interface IGameWorldConsoleCommandEndpoint
    {
        void SpawnItem(SpawnItemParams spawnItemParams);
        void SpawnMonster(SpawnMonsterParams spawnMonsterParams);
    }
}