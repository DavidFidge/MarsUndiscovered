using MarsUndiscovered.Game.Components;

namespace MarsUndiscovered.Interfaces
{
    public interface ISpawner
    {
        void Initialise(GameWorld gameWorld);
        void SpawnItem(SpawnItemParams spawnItemParams);
        void SpawnMonster(SpawnMonsterParams spawnMonsterParams);
        void SpawnMapExit(SpawnMapExitParams spawnMapExitParams);
        void SpawnMachine(SpawnMachineParams spawnMachineParams);
        void SpawnEnvironmentalEffect(SpawnEnvironmentalEffectParams spawnEnvironmentalEffectParams);
    }
}