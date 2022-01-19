using GoRogue.GameFramework;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Extensions;
using MarsUndiscovered.Messages;

namespace MarsUndiscovered.Components.Maps
{
    public class MonsterGenerator : BaseGameObjectGenerator, IMonsterGenerator
    {
        public Monster SpawnMonster(SpawnMonsterParams spawnMonsterParams, IGameObjectFactory gameObjectFactory, Map map, MonsterCollection monsterCollection)
        {
            var monster = gameObjectFactory
                .CreateMonster()
                .WithBreed(spawnMonsterParams.Breed)
                .PositionedAt(GetPosition(spawnMonsterParams, map))
                .AddToMap(map);

            monsterCollection.Add(monster.ID, monster);

            Mediator.Publish(new MapTileChangedNotification(monster.Position));

            return monster;
        }
    }
}
