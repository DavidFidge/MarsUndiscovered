using GoRogue.GameFramework;

namespace MarsUndiscovered.Components.Maps
{
    public interface IMonsterGenerator
    {
        Monster SpawnMonster(SpawnMonsterParams spawnMonsterParams, Map map, MonsterCollection monsterCollection);
    }
}