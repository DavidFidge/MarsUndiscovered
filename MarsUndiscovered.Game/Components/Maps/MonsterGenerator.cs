﻿using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Game.Messages;

namespace MarsUndiscovered.Game.Components.Maps
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

            monsterCollection.Add(monster.ID, monster);

            Mediator.Publish(new MapTileChangedNotification(monster.Position));

            return monster;
        }
    }
}