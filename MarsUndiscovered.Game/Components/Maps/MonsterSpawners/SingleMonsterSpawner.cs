namespace MarsUndiscovered.Game.Components.Maps;

public class SingleMonsterSpawner : MonsterSpawner
{
    private readonly Breed _breed;

    public SingleMonsterSpawner(IMonsterGenerator monsterGenerator, GameWorld gameWorld, Breed breed) : base(monsterGenerator, gameWorld)
    {
        _breed = breed;
    }
        
    public override void Spawn(MarsMap map)
    {
        SpawnMonster(new SpawnMonsterParams().WithBreed(_breed).OnMap(map.Id));
    }
}