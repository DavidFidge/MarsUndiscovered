using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps.MapPointChoiceRules;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Game.ViewMessages;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class MonsterGenerator : BaseGameObjectGenerator, IMonsterGenerator
    {
        public void SpawnMonster(SpawnMonsterParams spawnMonsterParams, GameWorld gameWorld)
        {
            spawnMonsterParams.Result = null;
            var map = gameWorld.Maps.Single(m => m.Id == spawnMonsterParams.MapId);

            var monster = gameWorld.GameObjectFactory
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
            
            if (position == Point.None)
                return;
            
            monster.PositionedAt(position);

            monster.AddToMap(map);

            gameWorld.Monsters.Add(monster.ID, monster);

            if (spawnMonsterParams.LeaderId.HasValue)
                monster.SetLeader(gameWorld.Monsters[spawnMonsterParams.LeaderId.Value]);
            
            monster.MonsterState = spawnMonsterParams.MonsterState;

            monster.AllegianceCategory = spawnMonsterParams.AllegianceCategory;
            
            Mediator.Publish(new MapTileChangedNotification(monster.Position));

            spawnMonsterParams.Result = monster;
        }
    }
}
