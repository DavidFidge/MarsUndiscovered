using GoRogue.Random;
using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class SaveLoadGameTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void Should_Save_Then_Load_Game()
        {
            // Arrange
            NewGameWithNoMonstersNoItems();
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach"));
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
            Assert.AreEqual(_gameWorld.GameObjects.Values.OfType<MarsGameObject>().Count(), newGameWorld.GameObjects.Values.OfType<MarsGameObject>().Count());
            Assert.AreEqual(1, newGameWorld.GameObjects.Values.OfType<Monster>().Count());
            Assert.AreEqual(1, newGameWorld.GameObjects.Values.OfType<Item>().Count());
            Assert.AreEqual(1, newGameWorld.GameObjects.Values.OfType<Player>().Count());
            Assert.AreEqual(2, newGameWorld.GetRadioCommsItemsSince(0).Count);
            Assert.AreEqual(0, newGameWorld.Inventory.Items.Count);

            var mapEntities = newGameWorld.CurrentMap.Entities.ToList();

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
            
            var item1 = SpawnItemAndAddToInventory(ItemType.MagnesiumPipe);
            var item2 = SpawnItemAndAddToInventory(ItemType.MagnesiumPipe);
            var item3 = SpawnItemAndAddToInventory(ItemType.HealingBots);
            var item4 = SpawnItemAndAddToInventory(ItemType.HealingBots);
            var item5 = SpawnItemAndEquip(ItemType.IronSpike);
            
            _gameWorld.Inventory.ItemTypeDiscoveries[ItemType.HealingBots].IsItemTypeDiscovered = true;

            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);
            
            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            Assert.AreEqual(5, newGameWorld.Inventory.Items.Count);
            CollectionAssert.AreEquivalent(newGameWorld.Items.Values, newGameWorld.Inventory.Items);

            Assert.AreEqual(4, newGameWorld.Inventory.ItemKeyAssignments.Count);
            CollectionAssert.AreEquivalent(newGameWorld.Items.Values, newGameWorld.Inventory.ItemKeyAssignments.SelectMany(i => i.Value).ToList());
            Assert.IsTrue(newGameWorld.Inventory.ItemTypeDiscoveries.Count > 0);

            var itemTypeDiscovery = newGameWorld.Inventory.ItemTypeDiscoveries[ItemType.HealingBots];
            Assert.IsTrue(itemTypeDiscovery.IsItemTypeDiscovered);
            Assert.IsNull(newGameWorld.Player.MeleeAttack);
            Assert.IsNotNull(newGameWorld.Player.LineAttack);
        }

        [TestMethod]
        public void Should_Save_Then_Load_Game_MapSeenTiles()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();

            _gameWorld.Player.Position = new Point(0, 0);
            var wallPosition = new Point(1, 1);
            _gameWorld.CreateWall(wallPosition);

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(2, 2)));
            var monster = _gameWorld.Monsters.First().Value;

            _gameWorld.UpdateFieldOfView();

            _gameWorld.Player.Position = new Point(0, 1);
            _gameWorld.UpdateFieldOfView();
            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.UpdateFieldOfView();
            monster.Position = new Point(3, 3);
            _gameWorld.UpdateFieldOfView();

            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            Assert.IsTrue(newGameWorld.CurrentMap.SeenTiles[new Point(2, 2)].HasBeenSeen);
            Assert.IsTrue(newGameWorld.CurrentMap.SeenTiles[new Point(3, 3)].HasBeenSeen);

            Assert.IsFalse(newGameWorld.CurrentMap.PlayerFOV.CurrentFOV.Contains(new Point(2, 2)));
            Assert.IsFalse(newGameWorld.CurrentMap.PlayerFOV.CurrentFOV.Contains(new Point(3, 3)));

            Assert.IsTrue(newGameWorld.CurrentMap.SeenTiles[new Point(2, 2)].LastSeenGameObjects.Any(m => m.ID == monster.ID));
            Assert.IsFalse(newGameWorld.CurrentMap.SeenTiles[new Point(3, 3)].LastSeenGameObjects.Any(m => m.ID == monster.ID));

            Assert.IsNull(newGameWorld.CurrentMap.GetObjectAt<Monster>(2, 2));
            Assert.IsNotNull(newGameWorld.CurrentMap.GetObjectAt<Monster>(3, 3));
        }

        [TestMethod]
        public void Should_Save_Then_Load_Game_Multiple_Maps()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();

            var maps = _gameWorld.Maps.ToList();

            _gameWorld.Player.Position = new Point(0, 0);

            _gameWorld.SpawnMonster(new SpawnMonsterParams().OnMap(maps[0].Id).WithBreed("Roach").AtPosition(new Point(2, 2)));
            _gameWorld.SpawnMonster(new SpawnMonsterParams().OnMap(maps[1].Id).WithBreed("Roach").AtPosition(new Point(2, 2)));

            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            Assert.AreEqual(2, newGameWorld.Maps.Count);
            var newGameMaps = newGameWorld.Maps.ToList();
            Assert.AreEqual(2, newGameMaps.Count);
            Assert.AreEqual(maps[0].Id, newGameMaps[0].Id);
            Assert.AreEqual(maps[1].Id, newGameMaps[1].Id);

            Assert.AreEqual(newGameMaps[0], newGameWorld.CurrentMap);
            Assert.AreEqual(newGameMaps[0], newGameWorld.Player.CurrentMap);

            var newGameMonsterMap1 = newGameWorld.Monsters.First().Value;
            var newGameMonsterMap2 = newGameWorld.Monsters.Skip(1).First().Value;

            Assert.AreEqual(newGameMonsterMap1.CurrentMap, newGameMaps[0]);
            Assert.AreEqual(newGameMonsterMap2.CurrentMap, newGameMaps[1]);

            Assert.IsNotNull(newGameMaps[0].GetObjectAt<Monster>(2, 2));
            Assert.IsNotNull(newGameMaps[1].GetObjectAt<Monster>(2, 2));
            Assert.AreNotSame(newGameMaps[0].GetObjectAt<Monster>(2, 2), newGameMaps[1].GetObjectAt<Monster>(2, 2));
        }

        [TestMethod]
        public void Should_Save_Then_Load_Game_MapExits()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();

            var maps = _gameWorld.Maps.ToList();

            _gameWorld.Player.Position = new Point(0, 0);

            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            Assert.AreEqual(2, newGameWorld.MapExits.Count);

            var mapExit1 = newGameWorld.MapExits.Values.First(m => m.CurrentMap.Equals(newGameWorld.CurrentMap));
            var mapExit2 = newGameWorld.MapExits.Values.First(m => !m.CurrentMap.Equals(newGameWorld.CurrentMap));

            Assert.AreEqual(mapExit2, mapExit1.Destination);
            Assert.AreEqual(mapExit1, mapExit2.Destination);

            Assert.AreEqual(Direction.Down, mapExit1.Direction);
            Assert.AreEqual(Direction.Up, mapExit2.Direction);

            Assert.AreNotEqual(Point.None, mapExit1.LandingPosition);
            Assert.AreNotEqual(Point.None, mapExit2.LandingPosition);
        }
    }
}
