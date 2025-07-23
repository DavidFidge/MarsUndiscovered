using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.Maps;

public class ConstantGroupMonsterSpawner : MonsterSpawner
{
    private readonly List<Breed> _breeds;

    public ConstantGroupMonsterSpawner(IMonsterGenerator monsterGenerator, GameWorld gameWorld, List<Breed> breeds) : base(
        monsterGenerator, gameWorld)
    {
        _breeds = breeds;
    }

    public override void Spawn(MarsMap map)
    {
        var point = Point.None;
        Monster leader = null;
            
        foreach (var breed in _breeds)
        {
            var spawnMonsterParams = new SpawnMonsterParams().AtPosition(point).WithBreed(breed).WithState(MonsterState.Wandering).OnMap(map.Id);
            
            if (leader != null)
                spawnMonsterParams.WithLeader(leader.ID);
            
            var monster = SpawnMonster(spawnMonsterParams);

            if (leader == null)
                leader = monster;
            
            point = monster.Position;
        }
    }
}