﻿using MarsUndiscovered.Game.Components.Factories;

namespace MarsUndiscovered.Game.Components.Maps
{
    public interface IMonsterGenerator
    {
        Monster SpawnMonster(SpawnMonsterParams spawnMonsterParams, IGameObjectFactory gameObjectFactory, MapCollection maps, MonsterCollection monsterCollection);
    }
}