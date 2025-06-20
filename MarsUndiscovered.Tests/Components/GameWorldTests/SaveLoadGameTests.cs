using GoRogue.Random;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class SaveLoadGameTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void Should_Save_Then_Load_Game_Specific_Entities()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach"));
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            _gameWorld.SpawnMachine(new SpawnMachineParams().WithMachineType(MachineType.Analyzer));
            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            Assert.AreNotSame(_gameWorld, newGameWorld);

            Assert.AreEqual(_gameWorld.Seed, newGameWorld.Seed);
            Assert.AreEqual(_gameWorld.GameObjectFactory.LastId, newGameWorld.GameObjectFactory.LastId);

            var gameWorldObjectsByType = _gameWorld.GameObjects
                .GroupBy(go => go.Value.GetType())
                .ToDictionary(g => g.Key, g => g.ToList());
            
            var newGameWorldObjectsByType = newGameWorld.GameObjects
                .GroupBy(go => go.Value.GetType())
                .ToDictionary(g => g.Key, g => g.ToList());
            
            Assert.AreEqual(gameWorldObjectsByType.Count, newGameWorldObjectsByType.Count);
            Assert.AreEqual(_gameWorld.GameObjects.Count, newGameWorld.GameObjects.Count);
            Assert.AreEqual(_gameWorld.GameObjects.Values.OfType<MarsGameObject>().Count(), newGameWorld.GameObjects.Values.OfType<MarsGameObject>().Count());
            Assert.AreEqual(1, newGameWorld.GameObjects.Values.OfType<Monster>().Count());
            Assert.AreEqual(1, newGameWorld.GameObjects.Values.OfType<Item>().Count());
            Assert.AreEqual(1, newGameWorld.GameObjects.Values.OfType<Player>().Count());
            Assert.AreEqual(1, newGameWorld.GameObjects.Values.OfType<Machine>().Count());
            Assert.AreEqual(2, newGameWorld.GetNewRadioCommsItems().Count);
            Assert.AreEqual(0, newGameWorld.Inventory.Items.Count);

            var mapEntities = newGameWorld.CurrentMap.Entities.ToList();

            Assert.IsTrue(mapEntities.Select(s => s.Item).Contains(newGameWorld.GameObjects.Values.OfType<Player>().First()));
            Assert.IsTrue(mapEntities.Select(s => s.Item).Contains(newGameWorld.GameObjects.Values.OfType<Item>().First()));
            Assert.IsTrue(mapEntities.Select(s => s.Item).Contains(newGameWorld.GameObjects.Values.OfType<Monster>().First()));
            Assert.IsTrue(mapEntities.Select(s => s.Item).Contains(newGameWorld.GameObjects.Values.OfType<Machine>().First()));
            
            Assert.AreEqual(_gameWorld.Player.ID, newGameWorld.Player.ID);
            Assert.AreEqual(_gameWorld.Player.Position, newGameWorld.Player.Position);
            Assert.AreEqual(_gameWorld.Player.IsWalkable, newGameWorld.Player.IsWalkable);
            Assert.AreEqual(_gameWorld.Player.IsTransparent, newGameWorld.Player.IsTransparent);
        }

        [TestMethod]
        public void Should_Save_Then_Load_Game_Monsters_With_Leaders()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);
            var monsterLeader = new SpawnMonsterParams().WithBreed("RepairDroid");
            _gameWorld.SpawnMonster(monsterLeader);

            var monster = new SpawnMonsterParams().WithBreed("RepairDroid").WithLeader(monsterLeader.Result.ID);
            _gameWorld.SpawnMonster(monster);

            Assert.IsNotNull(monster.Result.Leader);
            Assert.AreEqual(monsterLeader.Result, monster.Result.Leader);
            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            Assert.AreNotSame(_gameWorld, newGameWorld);
            Assert.IsNotNull(newGameWorld.Monsters[monster.Result.ID].Leader);
            Assert.AreEqual(monsterLeader.Result.ID, newGameWorld.Monsters[monster.Result.ID].Leader.ID);
        }
        
        [TestMethod]
        public void Should_Not_Reset_Seen_RadioCommsItems()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);

            _gameWorld.GetNewRadioCommsItems();
            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            Assert.AreEqual(0, newGameWorld.GetNewRadioCommsItems().Count);
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
            NewGameWithTestLevelGenerator(_gameWorld);
            
            var item1 = SpawnItemAndAddToInventory(_gameWorld, ItemType.MagnesiumPipe);
            var item2 = SpawnItemAndAddToInventory(_gameWorld, ItemType.MagnesiumPipe);
            var item3 = SpawnItemAndAddToInventory(_gameWorld, ItemType.HealingBots);
            var item4 = SpawnItemAndAddToInventory(_gameWorld, ItemType.HealingBots);
            var item5 = SpawnItemAndEquip(_gameWorld, ItemType.IronSpike);
            
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

            var loadedItem5 = newGameWorld.Inventory.EquippedWeapon;
            Assert.AreEqual(item5.ID, loadedItem5.ID);
            Assert.IsTrue(loadedItem5.IsEquipped);
        }

        [TestMethod]
        public void Should_Save_Then_Load_Game_MapSeenTiles()
        {
            // Arrange
            var wallPosition = new Point(1, 1);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition });
            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 0));

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
            var wallPosition = new Point(0, 0);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator);

            var maps = _gameWorld.Maps.ToList();

            var playerPosition = GlobalRandom.DefaultRNG.RandomPosition(maps[0], MapHelpers.EmptyPointOnFloor);
            var monsterPosition1 = GlobalRandom.DefaultRNG.RandomPosition(maps[0], MapHelpers.EmptyPointOnFloor);
            var monsterPosition2 = GlobalRandom.DefaultRNG.RandomPosition(maps[1], MapHelpers.EmptyPointOnFloor);

            _gameWorld.Player.Position = playerPosition;
            _gameWorld.SpawnMonster(new SpawnMonsterParams().OnMap(maps[0].Id).WithBreed("Roach").AtPosition(monsterPosition1));
            _gameWorld.SpawnMonster(new SpawnMonsterParams().OnMap(maps[1].Id).WithBreed("Roach").AtPosition(monsterPosition2));

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

            Assert.IsNotNull(newGameMaps[0].GetObjectAt<Monster>(monsterPosition1));
            Assert.IsNotNull(newGameMaps[1].GetObjectAt<Monster>(monsterPosition2));
            Assert.AreNotSame(newGameMaps[0].GetObjectAt<Monster>(monsterPosition1), newGameMaps[1].GetObjectAt<Monster>(monsterPosition2));
        }

        [TestMethod]
        public void Should_Save_Then_Load_Game_MapExits()
        {
            // Arrange
            var wallPosition = new Point(1, 0);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, addExits: true, playerPosition: new Point(0, 0));

            var maps = _gameWorld.Maps.ToList();

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

            Assert.AreEqual(MapExitType.MapExitDown, mapExit1.MapExitType);
            Assert.AreEqual(MapExitType.MapExitUp, mapExit2.MapExitType);

            Assert.AreNotEqual(Point.None, mapExit1.LandingPosition);
            Assert.AreNotEqual(Point.None, mapExit2.LandingPosition);
        }
        
        [TestMethod]
        public void Should_Save_Then_Load_Game_Weapon_LineAttack_Properties_Restored()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            var maps = _gameWorld.Maps.ToList();

            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.IronSpike).AtPosition(new Point(1, 1))
                .OnMap(_gameWorld.CurrentMap.Id));

            var item = _gameWorld.Items.Values.First();

            Assert.IsTrue(item.LineAttack.DamageRange.Min > 0);
            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            var newItem = newGameWorld.Items.Values.First();
            Assert.AreEqual(item.LineAttack.DamageRange.Min, newItem.LineAttack.DamageRange.Min);
            Assert.AreEqual(item.LineAttack.DamageRange.Max, newItem.LineAttack.DamageRange.Max);
        }

        [TestMethod]
        public void Should_Save_Then_Load_Game_Enchanted_Weapon_Properties_Restored()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            var spawnItemParams = new SpawnItemParams()
                .WithItemType(ItemType.IronSpike)
                .IntoPlayerInventory();
            
            _gameWorld.SpawnItem(spawnItemParams);
            
            var item = spawnItemParams.Result;

            var currentDamageMin = item.LineAttack.DamageRange.Min;
            _gameWorld.EnchantItemRequest(_gameWorld.Inventory.GetKeyForItem(item));

            Assert.IsTrue(item.LineAttack.DamageRange.Min > currentDamageMin);

            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            var newItem = newGameWorld.Items.Values.First();
            Assert.AreEqual(item.LineAttack.DamageRange.Min, newItem.LineAttack.DamageRange.Min);
            Assert.AreEqual(item.LineAttack.DamageRange.Max, newItem.LineAttack.DamageRange.Max);
        }
        
        [TestMethod]
        public void Should_Retain_Residual_Regen()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach"));
            _gameWorld.Player.ResidualRegen = 0.1m;
            _gameWorld.Monsters.First().Value.ResidualRegen = 0.2m;

            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            Assert.AreEqual(0.1m, newGameWorld.Player.ResidualRegen);
            Assert.AreEqual(0.2m, newGameWorld.Monsters.First().Value.ResidualRegen);
        }
    }
}
