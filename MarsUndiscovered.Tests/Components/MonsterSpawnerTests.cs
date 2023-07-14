using FrigidRogue.MonoGame.Core.Extensions;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Maps;
using ShaiRandom.Collections;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Tests.Components;

[TestClass]
public class MonsterSpawnerTests : BaseGameWorldIntegrationTests
{
    [TestMethod]
    public void SingleMonsterSpawner_Should_Spawn_Single_Monster()
    {
        NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);

        var map = _gameWorld.Maps.First();
        var monsterGenerator = new MonsterGenerator();
        SetupBaseComponent(monsterGenerator);
        
        var breed = Breed.GetBreed("Roach");

        var monsterSpawner = new SingleMonsterSpawner(monsterGenerator, _gameWorld, breed);
        monsterSpawner.Spawn(map);

        var monster = _gameWorld.Monsters.Single().Value;
        Assert.AreEqual(breed, monster.Breed);
        
        Assert.IsTrue(monster.Leader == null);
    }
    
    [TestMethod]
    public void VariableCountMonsterSpawner_Should_Spawn_Multiple_Monsters()
    {
        NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);

        var map = _gameWorld.Maps.First();
        var monsterGenerator = new MonsterGenerator();
        SetupBaseComponent(monsterGenerator);
        
        var breed = Breed.GetBreed("Roach");

        var random = new KnownSeriesRandom(new[]
        {
            2
        });

        var monsterSpawner = new VariableCountMonsterSpawner(monsterGenerator, _gameWorld, breed, random, 1, 3);
        monsterSpawner.Spawn(map);
        
        var monsters = _gameWorld.Monsters.Select(m => m.Value).ToList();
        Assert.AreEqual(2, monsters.Count);
        
        Assert.IsTrue(monsters.All(m => m.Breed == breed));
        
        var leader = monsters.Single(m => m.Leader == null);
        Assert.IsTrue(monsters.Where(m => m.Leader != null).All(m => m.Leader == leader));
    }
    
    [TestMethod]
    public void ConstantGroupMonsterSpawner_Should_Spawn_A_Group_OfMonsters_With_Given_Breeds_Next_To_Each_Other()
    {
        NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);

        var map = _gameWorld.Maps.First();
        var monsterGenerator = new MonsterGenerator();
        SetupBaseComponent(monsterGenerator);
        
        var breed1 = Breed.GetBreed("Roach");
        var breed2 = Breed.GetBreed("RepairDroid");
        var breed3 = Breed.GetBreed("Werewolf");
        
        var breeds = new List<Breed> { breed1, breed2, breed3 };
        
        var monsterSpawner = new ConstantGroupMonsterSpawner(monsterGenerator, _gameWorld, breeds);
        monsterSpawner.Spawn(map);

        var monsters = _gameWorld.Monsters.Select(m => m.Value).ToList();
        
        Assert.AreEqual(3, monsters.Count);
        
        Assert.AreEqual(breed1, monsters[0].Breed);
        Assert.AreEqual(breed2, monsters[1].Breed);
        Assert.AreEqual(breed3, monsters[2].Breed);

        Assert.IsTrue(monsters[0].Position.Neighbours().Contains(monsters[1].Position));
        Assert.IsTrue(monsters[1].Position.Neighbours().Contains(monsters[2].Position));
    
        var leader = monsters.Single(m => m.Leader == null);
        Assert.IsTrue(monsters.Where(m => m.Leader != null).All(m => m.Leader == leader));
    }

    [TestMethod]
    public void VariableCountAndTypeMonsterSpawner_Should_Spawn_A_Group_Of_Monsters_With_Different_Types()
    {
        NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);

        var map = _gameWorld.Maps.First();
        var monsterGenerator = new MonsterGenerator();
        SetupBaseComponent(monsterGenerator);
        
        var breed1 = Breed.GetBreed("Roach");
        var breed2 = Breed.GetBreed("RepairDroid");
        
        var random = new KnownSeriesRandom(intSeries:new[]
            {
                3 // 3 monsters
            },
            ulongSeries: new []{ ulong.MaxValue, 0u , 0u } // RepairDroid, Roach, Roach
        );

        var probabilityTable = new ProbabilityTable<Breed>(new List<(Breed item, double weight)>
        {
            (breed1, 1),
            (breed2, 1)
        });
        
        var monsterSpawner = new VariableCountAndTypeMonsterSpawner(monsterGenerator, _gameWorld, random, 1, 4, probabilityTable);
        
        monsterSpawner.Spawn(map);

        var monsters = _gameWorld.Monsters.Select(m => m.Value).ToList();
        
        Assert.AreEqual(3, monsters.Count);
        
        Assert.AreEqual(breed2, monsters[0].Breed);
        Assert.AreEqual(breed1, monsters[1].Breed);
        Assert.AreEqual(breed1, monsters[2].Breed);

        Assert.IsTrue(monsters[0].Position.Neighbours().Contains(monsters[1].Position));
        Assert.IsTrue(monsters[1].Position.Neighbours().Contains(monsters[2].Position));
    
        var leader = monsters.Single(m => m.Leader == null);
        Assert.IsTrue(monsters.Where(m => m.Leader != null).All(m => m.Leader == leader));
    }
}