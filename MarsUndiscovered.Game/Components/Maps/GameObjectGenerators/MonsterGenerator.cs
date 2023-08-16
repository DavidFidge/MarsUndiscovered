using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps.MapPointChoiceRules;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Game.ViewMessages;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class MonsterGenerator : BaseGameObjectGenerator, IMonsterGenerator
    {
        public void SpawnMonster(SpawnMonsterParams spawnMonsterParams, IGameObjectFactory gameObjectFactory, MapCollection maps, MonsterCollection monsterCollection)
        {
            spawnMonsterParams.Result = null;
            var map = maps.Single(m => m.Id == spawnMonsterParams.MapId);

            var monster = gameObjectFactory
                .CreateGameObject<Monster>()
                .WithBreed(spawnMonsterParams.Breed);

            if (monster.IsWallTurret)
            {
                spawnMonsterParams.MapPointChoiceRules.Add(new WallAdjacentToFloorRule());
            }
            else
            {
                spawnMonsterParams.MapPointChoiceRules.Add(new EmptyFloorRule());
            }

            spawnMonsterParams.AssignMap(map);

            var position = GetPosition(spawnMonsterParams, map);
            
            monster.PositionedAt(position);

            monster.AddToMap(map);

            monsterCollection.Add(monster.ID, monster);

            if (spawnMonsterParams.LeaderId.HasValue)
                monster.SetLeader(monsterCollection[spawnMonsterParams.LeaderId.Value]);
            
            monster.MonsterState = spawnMonsterParams.MonsterState;
            
            Mediator.Publish(new MapTileChangedNotification(monster.Position));

            spawnMonsterParams.Result = monster;
        }
    }
}
