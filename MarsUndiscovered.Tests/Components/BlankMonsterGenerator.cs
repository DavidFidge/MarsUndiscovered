using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps;

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