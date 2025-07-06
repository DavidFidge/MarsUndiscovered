namespace MarsUndiscovered.Game.Components.Maps;

public abstract class MonsterSpawner
{
    private readonly IMonsterGenerator _monsterGenerator;
    private readonly GameWorld _gameWorld;

    public MonsterSpawner(IMonsterGenerator monsterGenerator, GameWorld gameWorld)
    {
        _monsterGenerator = monsterGenerator;
        _gameWorld = gameWorld;
    }

    protected Monster SpawnMonster(SpawnMonsterParams spawnMonsterParams)
    {
        _monsterGenerator.SpawnMonster(spawnMonsterParams, _gameWorld);
        return spawnMonsterParams.Result;
    }
        
    public abstract void Spawn(MarsMap map);
}