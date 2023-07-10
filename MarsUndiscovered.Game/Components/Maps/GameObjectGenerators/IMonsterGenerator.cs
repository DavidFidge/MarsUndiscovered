using MarsUndiscovered.Game.Components.Factories;

namespace MarsUndiscovered.Game.Components.Maps
{
    public interface IMonsterGenerator
    {
        void SpawnMonster(SpawnMonsterParams spawnMonsterParams, IGameObjectFactory gameObjectFactory, MapCollection maps, MonsterCollection monsterCollection);
    }
}