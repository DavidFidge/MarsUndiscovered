using GoRogue.GameFramework;

using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Maps;

namespace MarsUndiscovered.Tests.Components
{
    public class BlankMonsterGenerator : IMonsterGenerator
    {
        public IMonsterGenerator OriginalMonsterGenerator { get; }

        public BlankMonsterGenerator(IMonsterGenerator originalMonsterGenerator)
        {
            OriginalMonsterGenerator = originalMonsterGenerator;
        }

        public Monster SpawnMonster(SpawnMonsterParams spawnMonsterParams, Map map, MonsterCollection monsterCollection)
        {
            return null;
        }
    }
}