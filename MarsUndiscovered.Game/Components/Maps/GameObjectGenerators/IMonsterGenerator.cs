using MarsUndiscovered.Game.Components.Factories;

namespace MarsUndiscovered.Game.Components.Maps
{
    public interface IMonsterGenerator
    {
        void SpawnMonster(SpawnMonsterParams spawnMonsterParams, GameWorld gameWorld);
    }
}