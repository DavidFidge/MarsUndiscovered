using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Game.ViewMessages;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class MonsterGenerator : BaseGameObjectGenerator, IMonsterGenerator
    {
        public Monster SpawnMonster(SpawnMonsterParams spawnMonsterParams, IGameObjectFactory gameObjectFactory, MapCollection maps, MonsterCollection monsterCollection)
        {
            var map = maps.Single(m => m.Id == spawnMonsterParams.MapId);

            var monster = gameObjectFactory
                .CreateGameObject<Monster>()
                .WithBreed(spawnMonsterParams.Breed);

            if (monster.IsWallTurret)
                monster.PositionedAt(GetWallPositionAdjacentToFloor(spawnMonsterParams, map));
            else
                monster.PositionedAt(GetPosition(spawnMonsterParams, map));

            monster.AddToMap(map);

            monsterCollection.Add(monster.ID, monster);

            Mediator.Publish(new MapTileChangedNotification(monster.Position));

            return monster;
        }
    }
}
