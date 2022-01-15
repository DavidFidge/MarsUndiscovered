using System.Linq;

using GoRogue.Random;

using MarsUndiscovered.Commands;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class GameWorldIntegrationTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void Should_Resolve()
        {
            // Assert
            Assert.IsNotNull(_gameWorld);
        }

        [TestMethod]
        public void Should_Create_New_Game()
        {
            // Act
            _gameWorld.NewGame();

            // Assert
            Assert.IsNotNull(_gameWorld.Map);
            Assert.IsNotNull(_gameWorld.Player);
            Assert.IsNotNull(_gameWorld.GameObjects);
            Assert.IsTrue(_gameWorld.GameObjects.Count > 0);
            Assert.IsTrue(_gameWorld.Seed > 0);
            Assert.AreEqual(MapGenerator.MapWidth, _gameWorld.Map.Width);
            Assert.AreEqual(MapGenerator.MapHeight, _gameWorld.Map.Height);
        }

        [TestMethod]
        public void Should_Spawn_Monster()
        {
            // Arrange
            _gameWorld.NewGame();
            var currentMonsterCount = _gameWorld.Monsters.Count;

            // Act
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach));

            // Assert
            Assert.AreEqual(currentMonsterCount + 1, _gameWorld.Monsters.Count);
        }

        [TestMethod]
        public void Should_Spawn_Items()
        {
            // Arrange
            _gameWorld.NewGame();
            var currentItemCount = _gameWorld.Items.Count;

            // Act
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));

            // Assert
            Assert.AreEqual(currentItemCount + 1, _gameWorld.Items.Count);
        }

        [TestMethod]
        public void Should_Save_Then_Load_Game()
        {
            // Arrange
            NewGameWithNoMonstersNoItems();
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach));
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            Assert.AreNotSame(_gameWorld, newGameWorld);

            Assert.AreEqual(_gameWorld.Seed, newGameWorld.Seed);
            Assert.AreEqual(_gameWorld.GameObjectFactory.LastId, newGameWorld.GameObjectFactory.LastId);

            Assert.AreEqual(_gameWorld.GameObjects.Count, newGameWorld.GameObjects.Count);
            Assert.AreEqual(_gameWorld.GameObjects.Values.OfType<Wall>().Count(), newGameWorld.GameObjects.Values.OfType<Wall>().Count());
            Assert.AreEqual(_gameWorld.GameObjects.Values.OfType<Floor>().Count(), newGameWorld.GameObjects.Values.OfType<Floor>().Count());
            Assert.AreEqual(1, newGameWorld.GameObjects.Values.OfType<Monster>().Count());
            Assert.AreEqual(1, newGameWorld.GameObjects.Values.OfType<Item>().Count());
            Assert.AreEqual(1, newGameWorld.GameObjects.Values.OfType<Player>().Count());
            Assert.AreEqual(0, newGameWorld.Inventory.Items.Count);

            var mapEntities = newGameWorld.Map.Entities.ToList();

            Assert.IsTrue(mapEntities.Select(s => s.Item).Contains(newGameWorld.GameObjects.Values.OfType<Player>().First()));
            Assert.IsTrue(mapEntities.Select(s => s.Item).Contains(newGameWorld.GameObjects.Values.OfType<Item>().First()));
            Assert.IsTrue(mapEntities.Select(s => s.Item).Contains(newGameWorld.GameObjects.Values.OfType<Monster>().First()));

            Assert.AreEqual(_gameWorld.Player.ID, newGameWorld.Player.ID);
            Assert.AreEqual(_gameWorld.Player.Position, newGameWorld.Player.Position);
            Assert.AreEqual(_gameWorld.Player.IsWalkable, newGameWorld.Player.IsWalkable);
            Assert.AreEqual(_gameWorld.Player.IsTransparent, newGameWorld.Player.IsTransparent);
        }

        [TestMethod]
        public void Should_Retain_RandomNumber_Sequence()
        {
            // Arrange
            _gameWorld.NewGame();
            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);
            var nextRandomNumber = GlobalRandom.DefaultRNG.NextUInt();

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            var nextRandomNumberAfterLoad = GlobalRandom.DefaultRNG.NextUInt();
            Assert.AreEqual(nextRandomNumber, nextRandomNumberAfterLoad);
        }

        [TestMethod]
        public void Should_Save_Then_Load_Game_Inventory()
        {
            // Arrange
            NewGameWithNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots));
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots));

            var item1 = _gameWorld.Items.First().Value;
            var item2 = _gameWorld.Items.Skip(1).First().Value;
            var item3 = _gameWorld.Items.Skip(2).First().Value;
            var item4 = _gameWorld.Items.Skip(3).First().Value;

            _gameWorld.Inventory.Add(item1);
            _gameWorld.Inventory.Add(item2);
            _gameWorld.Inventory.Add(item3);
            _gameWorld.Inventory.Add(item4);

            _gameWorld.Inventory.ItemTypeDiscoveries[ItemType.HealingBots].IsItemTypeDiscovered = true;

            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            Assert.AreEqual(4, newGameWorld.Inventory.Items.Count);
            CollectionAssert.AreEquivalent(newGameWorld.Items.Values, newGameWorld.Inventory.Items);

            Assert.AreEqual(3, newGameWorld.Inventory.ItemKeyAssignments.Count);
            CollectionAssert.AreEquivalent(newGameWorld.Items.Values, newGameWorld.Inventory.ItemKeyAssignments.SelectMany(i => i.Value).ToList());
            Assert.IsTrue(newGameWorld.Inventory.ItemTypeDiscoveries.Count > 0);

            var itemTypeDiscovery = newGameWorld.Inventory.ItemTypeDiscoveries[ItemType.HealingBots];
            Assert.IsTrue(itemTypeDiscovery.IsItemTypeDiscovered);
        }

        [TestMethod]
        public void GetStatusOfMonstersInView_Should_Return_Monster_Statuses()
        {
            // Arrange
            NewGameWithNoWallsNoMonstersNoItems();

            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(0, 1)));
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(0, 2)));

            // Act
            var result = _gameWorld.GetStatusOfMonstersInView();

            // Assert
            Assert.AreEqual(2, result.Count);

            foreach (var monster in result)
            {
                Assert.AreEqual(_gameWorld.Monsters.Values.First().Health, monster.Health);
                Assert.AreEqual(_gameWorld.Monsters.Values.First().MaxHealth, monster.MaxHealth);
                Assert.AreEqual(_gameWorld.Monsters.Values.First().Name, monster.Name);
            }

            Assert.AreEqual(1, result[0].DistanceFromPlayer);
            Assert.AreEqual(2, result[1].DistanceFromPlayer);
        }

        [TestMethod]
        public void Monsters_Should_Stop_Acting_When_Player_Dies()
        {
            // Arrange
            NewGameWithNoWallsNoMonstersNoItems();

            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(0, 1)));
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(1, 0)));
            _gameWorld.Player.Health = 1;
            _gameWorld.RebuildGoalMaps();

            // Act
            var result = _gameWorld.MoveRequest(Direction.Down);

            // Assert
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result[0].Command is WalkCommand); // Player walks into monster
            Assert.IsTrue(result[1].Command is AttackCommand); // Player attacks first roach
            Assert.IsTrue(result[2].Command is AttackCommand); // A roach attacks player
            Assert.IsTrue(result[3].Command is DeathCommand); // Player dies. Second roach does not act.

            var deathCommand = (DeathCommand)result.Last().Command;

            Assert.AreEqual("killed by a roach", deathCommand.KilledByMessage);
        }
    }
}