using SadRogue.Primitives;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components.Maps;

public class VariableCountMonsterSpawner : MonsterSpawner
{
    private readonly Breed _breed;
    private readonly IEnhancedRandom _random;
    private readonly int _min;
    private readonly int _max;

    public VariableCountMonsterSpawner(IMonsterGenerator monsterGenerator, GameWorld gameWorld, Breed breed, IEnhancedRandom random, int min, int max) : base(monsterGenerator, gameWorld)
    {
        if (min > max)
            throw new ArgumentException("min must be less than or equal to max");

        _breed = breed;
        _random = random;
        _min = min;
        _max = max;
    }
        
    public override void Spawn(MarsMap map)
    {
        var count = _random.NextInt(_min, _max + 1);

        var point = Point.None;

        Monster leader = null;
        
        for (var i = 0; i < count; i++)
        {
            var spawnMonsterParams = new SpawnMonsterParams().AtPosition(point).WithBreed(_breed).WithState(MonsterState.Wandering).OnMap(map.Id);

            if (leader != null)
                spawnMonsterParams.WithLeader(leader.ID);
            
            var monster = SpawnMonster(spawnMonsterParams);

            if (leader == null)
                leader = monster;
            
            point = monster.Position;
        }
    }
}