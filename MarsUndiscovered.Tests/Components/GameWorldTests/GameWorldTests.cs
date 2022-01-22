using System.Linq;

using GoRogue.Random;

using MarsUndiscovered.Commands;
using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class GameWorldTests : BaseGameWorldIntegrationTests
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
            Assert.IsNotNull(_gameWorld.CurrentMap);
            Assert.IsNotNull(_gameWorld.Player);
            Assert.IsNotNull(_gameWorld.GameObjects);
            Assert.IsTrue(_gameWorld.GameObjects.Count > 0);
            Assert.IsTrue(_gameWorld.MapExits.Count > 0);
            Assert.IsTrue(_gameWorld.Seed > 0);
            Assert.AreEqual(MarsMap.MapWidth, _gameWorld.CurrentMap.Width);
            Assert.AreEqual(MarsMap.MapHeight, _gameWorld.CurrentMap.Height);
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
            Assert.AreEqual(_gameWorld.GameObjects.Values.OfType<MarsGameObject>().Count(), newGameWorld.GameObjects.Values.OfType<MarsGameObject>().Count());
            Assert.AreEqual(1, newGameWorld.GameObjects.Values.OfType<Monster>().Count());
            Assert.AreEqual(1, newGameWorld.GameObjects.Values.OfType<Item>().Count());
            Assert.AreEqual(1, newGameWorld.GameObjects.Values.OfType<Player>().Count());
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
            NewGameWithCustomMapNoMonstersNoItems();

            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(0, 1)));
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(0, 2)));
            _gameWorld.UpdateFieldOfView(false);

            // Act
            var result = _gameWorld.GetStatusOfMonstersInView();

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void GetStatusOfMonstersInView_Should_Return_Monsters_In_Field_Of_View_Only()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();

            var wallPosition = new Point(1, 1);
            _gameWorld.CreateWall(wallPosition);
            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(2, 2)));
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(0, 2)));

            _gameWorld.UpdateFieldOfView(false);

            // Act
            var result = _gameWorld.GetStatusOfMonstersInView();

            // Assert
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void Monsters_Should_Stop_Acting_When_Player_Dies()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();

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

        [TestMethod]
        public void HasBeenSeen_Should_Be_False_For_Unseen_Tiles()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();

            _gameWorld.Player.Position = new Point(0, 0);
            var wallPosition = new Point(1, 1);
            _gameWorld.CreateWall(wallPosition);

            // Act
            _gameWorld.UpdateFieldOfView();

            // Assert
            Assert.IsFalse(_gameWorld.CurrentMap.SeenTiles[new Point(2, 2)].HasBeenSeen);
            Assert.IsTrue(_gameWorld.CurrentMap.SeenTiles[new Point(1, 0)].HasBeenSeen);
            Assert.IsTrue(_gameWorld.CurrentMap.SeenTiles[new Point(0, 1)].HasBeenSeen);

            Assert.IsFalse(_gameWorld.CurrentMap.PlayerFOV.CurrentFOV.Contains(new Point(2, 2)));
            Assert.IsTrue(_gameWorld.CurrentMap.PlayerFOV.CurrentFOV.Contains(new Point(1, 0)));
            Assert.IsTrue(_gameWorld.CurrentMap.PlayerFOV.CurrentFOV.Contains(new Point(0, 1)));
        }

        [TestMethod]
        public void HasBeenSeen_Should_Be_False_For_Unseen_Tiles_Should_Keep_GameObjects_That_Were_Last_Seen_There()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();

            _gameWorld.Player.Position = new Point(0, 0);
            var wallPosition = new Point(1, 1);
            _gameWorld.CreateWall(wallPosition);

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(2, 2)));
            var monster = _gameWorld.Monsters.First().Value;

            _gameWorld.UpdateFieldOfView();

            // Act
            _gameWorld.Player.Position = new Point(0, 1);
            _gameWorld.UpdateFieldOfView();
            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.UpdateFieldOfView();
            monster.Position = new Point(3, 3);
            _gameWorld.UpdateFieldOfView();

            // Assert
            Assert.IsTrue(_gameWorld.CurrentMap.SeenTiles[new Point(2, 2)].HasBeenSeen);
            Assert.IsTrue(_gameWorld.CurrentMap.SeenTiles[new Point(3, 3)].HasBeenSeen);

            Assert.IsFalse(_gameWorld.CurrentMap.PlayerFOV.CurrentFOV.Contains(new Point(2, 2)));
            Assert.IsFalse(_gameWorld.CurrentMap.PlayerFOV.CurrentFOV.Contains(new Point(3, 3)));

            Assert.IsTrue(_gameWorld.CurrentMap.SeenTiles[new Point(2, 2)].LastSeenGameObjects.Contains(monster));
            Assert.IsFalse(_gameWorld.CurrentMap.SeenTiles[new Point(3, 3)].LastSeenGameObjects.Contains(monster));

            Assert.IsNull(_gameWorld.CurrentMap.GetObjectAt<Monster>(new Point(2, 2)));
            Assert.IsNotNull(_gameWorld.CurrentMap.GetObjectAt<Monster>(new Point(3, 3)));
        }

        [TestMethod]
        public void Should_Save_Then_Load_Game_MapSeenTiles()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();

            _gameWorld.Player.Position = new Point(0, 0);
            var wallPosition = new Point(1, 1);
            _gameWorld.CreateWall(wallPosition);

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(2, 2)));
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

            Assert.IsNull(newGameWorld.CurrentMap.GetObjectAt<Monster>(new Point(2, 2)));
            Assert.IsNotNull(newGameWorld.CurrentMap.GetObjectAt<Monster>(new Point(3, 3)));
        }

        [TestMethod]
        public void Should_Save_Then_Load_Game_Multiple_Maps()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();

            var maps = _gameWorld.Maps.ToList();

            _gameWorld.Player.Position = new Point(0, 0);

            _gameWorld.SpawnMonster(new SpawnMonsterParams().OnMap(maps[0].Id).WithBreed(Breed.Roach).AtPosition(new Point(2, 2)));
            _gameWorld.SpawnMonster(new SpawnMonsterParams().OnMap(maps[1].Id).WithBreed(Breed.Roach).AtPosition(new Point(2, 2)));

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

            Assert.IsNotNull(newGameMaps[0].GetObjectAt<Monster>(new Point(2, 2)));
            Assert.IsNotNull(newGameMaps[1].GetObjectAt<Monster>(new Point(2, 2)));
            Assert.AreNotSame(newGameMaps[0].GetObjectAt<Monster>(new Point(2, 2)), newGameMaps[1].GetObjectAt<Monster>(new Point(2, 2)));
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