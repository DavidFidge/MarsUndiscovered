using GoRogue.GameFramework;

using MarsUndiscovered.Components.Factories;

namespace MarsUndiscovered.Components.Maps
{
    public interface IMonsterGenerator
    {
        Monster SpawnMonster(SpawnMonsterParams spawnMonsterParams, IGameObjectFactory gameObjectFactory, Map map, MonsterCollection monsterCollection);
    }
}