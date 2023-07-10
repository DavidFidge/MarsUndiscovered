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
            
        foreach (var breed in _breeds)
        {
            var monster = SpawnMonster(new SpawnMonsterParams().AtPosition(point).WithBreed(breed).OnMap(map.Id));
            point = monster.Position;
        }
    }
}