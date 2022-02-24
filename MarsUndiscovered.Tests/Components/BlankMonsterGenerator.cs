using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
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

        public Monster SpawnMonster(
            SpawnMonsterParams spawnMonsterParams,
            IGameObjectFactory gameObjectFactory,
            MarsMap map,
            MonsterCollection monsterCollection
        )
        {
            return null;
        }
    }
}