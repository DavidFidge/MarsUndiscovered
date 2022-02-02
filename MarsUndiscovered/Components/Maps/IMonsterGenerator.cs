using MarsUndiscovered.Components.Factories;

namespace MarsUndiscovered.Components.Maps
{
    public interface IMonsterGenerator
    {
        Monster SpawnMonster(SpawnMonsterParams spawnMonsterParams, IGameObjectFactory gameObjectFactory, MarsMap map, MonsterCollection monsterCollection);
    }
}