using SadRogue.Primitives;
using ShaiRandom.Collections;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components.Maps;

public class VariableCountAndTypeMonsterSpawner : MonsterSpawner
{
    private readonly IEnhancedRandom _random;
    private readonly ProbabilityTable<Breed> _probabilityTable;
    private readonly int _min;
    private readonly int _max;

    public VariableCountAndTypeMonsterSpawner(IMonsterGenerator monsterGenerator, GameWorld gameWorld, IEnhancedRandom random, int min, int max, ProbabilityTable<Breed> probabilityTable) : base(monsterGenerator, gameWorld)
    {
        if (min > max)
            throw new ArgumentException("min must be less than or equal to max");

        _random = random;
        _probabilityTable = probabilityTable;
        _probabilityTable.Random = _random;

        _min = min;
        _max = max;
    }
        
    public override void Spawn(MarsMap map)
    {
        var count = _random.NextInt(_min, _max + 1);
        var point = Point.None;
            
        for (var i = 0; i < count; i++)
        {
            var breed = _probabilityTable.NextItem();
                
            var monster = SpawnMonster(new SpawnMonsterParams().AtPosition(point).WithBreed(breed).OnMap(map.Id));
                
            point = monster.Position;
        }
    }
}