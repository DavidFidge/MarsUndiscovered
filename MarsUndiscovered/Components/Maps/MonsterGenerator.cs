using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.GameFramework;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Extensions;
using MarsUndiscovered.Messages;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components.Maps
{
    public class MonsterGenerator : BaseComponent, IMonsterGenerator
    {
        public IGameObjectFactory GameObjectFactory { get; set; }

        public Monster SpawnMonster(SpawnMonsterParams spawnMonsterParams, Map map, MonsterCollection monsterCollection)
        {
            var monster = GameObjectFactory
                .CreateMonster()
                .WithBreed(spawnMonsterParams.Breed)
                .PositionedAt(GetPosition(spawnMonsterParams, map))
                .AddToMap(map);

            monsterCollection.Add(monster.ID, monster);

            Mediator.Publish(new MapTileChangedNotification(monster.Position));

            return monster;
        }

        private Point GetPosition(SpawnMonsterParams spawnMonsterParams, Map map)
        {
            if (spawnMonsterParams.Position != null)
                return spawnMonsterParams.Position.Value;

            if (spawnMonsterParams.AvoidPosition != null)
                return map.RandomPositionAwayFrom(
                    spawnMonsterParams.AvoidPosition.Value,
                    spawnMonsterParams.AvoidPositionRange,
                    MapHelpers.EmptyPointOnFloor
                );

            return map.RandomPosition(MapHelpers.EmptyPointOnFloor);
        }
    }
}
