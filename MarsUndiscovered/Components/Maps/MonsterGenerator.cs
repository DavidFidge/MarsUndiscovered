using GoRogue.GameFramework;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Extensions;
using MarsUndiscovered.Messages;

namespace MarsUndiscovered.Components.Maps
{
    public class MonsterGenerator : BaseGameObjectGenerator, IMonsterGenerator
    {
        public Monster SpawnMonster(SpawnMonsterParams spawnMonsterParams, IGameObjectFactory gameObjectFactory, MarsMap map, MonsterCollection monsterCollection)
        {
            var monster = gameObjectFactory
                .CreateMonster()
                .WithBreed(spawnMonsterParams.Breed);

            if (monster.IsWallTurret)
                monster.PositionedAt(GetWallPositionAdjacentToFloor(spawnMonsterParams, map));
            else
                monster.PositionedAt(GetPosition(spawnMonsterParams, map));

            monster.AddToMap(map);

            monster.MonsterGoal.ChangeMap();

            monsterCollection.Add(monster.ID, monster);

            Mediator.Publish(new MapTileChangedNotification(monster.Position));

            return monster;
        }
    }
}
