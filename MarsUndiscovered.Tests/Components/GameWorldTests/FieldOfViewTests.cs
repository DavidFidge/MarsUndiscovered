using MarsUndiscovered.Game.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class FieldOfViewTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void GetStatusOfMonstersInView_Should_Return_Monster_Statuses()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 1)));
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 2)));
            _gameWorld.TestResetFieldOfView();

            // Act
            var result = _gameWorld.GetStatusOfMonstersInView();

            // Assert
            Assert.AreEqual(2, result.Count);
        }
        
        [TestMethod]
        public void GetStatusOfMonstersInView_Should_Return_Monster_Statuses_In_Visual_Range()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));
            _gameWorld.Player.VisualRange = 1;
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 1)));
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 2)));
            _gameWorld.TestResetFieldOfView();

            // Act
            var result = _gameWorld.GetStatusOfMonstersInView();

            // Assert
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void GetStatusOfMonstersInView_Should_Return_Monsters_In_Field_Of_View_Only()
        {
            // Arrange
            var wallPosition = new Point(1, 1);

            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 0));

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(2, 2)));
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 2)));

            _gameWorld.TestResetFieldOfView();

            // Act
            var result = _gameWorld.GetStatusOfMonstersInView();

            // Assert
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void HasBeenSeen_Should_Be_False_For_Unseen_Tiles()
        {
            // Arrange
            var wallPosition = new Point(1, 1);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 0));

            // Act
            _gameWorld.TestResetFieldOfView();

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
            var wallPosition = new Point(1, 1);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 0));

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(2, 2)));
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

            Assert.IsNull(_gameWorld.CurrentMap.GetObjectAt<Monster>(2, 2));
            Assert.IsNotNull(_gameWorld.CurrentMap.GetObjectAt<Monster>(3, 3));
        }
    }
}
