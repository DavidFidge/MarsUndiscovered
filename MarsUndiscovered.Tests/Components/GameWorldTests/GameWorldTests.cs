using MarsUndiscovered.Game.Components;

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
            Assert.IsTrue(_gameWorld.Monsters.Count > 0);
            Assert.IsTrue(_gameWorld.Items.Count > 0);
            Assert.IsTrue(_gameWorld.MapExits.Count > 0);
            Assert.IsTrue(_gameWorld.Machines.Count > 0);
            Assert.IsTrue(_gameWorld.Seed > 0);
            
            // We can't get exact expected map widths and heights as each map generator is different,
            // so just test that they are sensible values
            Assert.IsTrue(_gameWorld.CurrentMap.Width >= 10);
            Assert.IsTrue(_gameWorld.CurrentMap.Width < 200);
            Assert.IsTrue(_gameWorld.CurrentMap.Height >= 10);
            Assert.IsTrue(_gameWorld.CurrentMap.Height < 200);
        }
        
        [TestMethod]
        public void Should_Force_Move()
        {
            // Arrange
            _gameWorld.NewGame();

            // Act
            _gameWorld.ForceLevelChange(ForceLevelChange.NextLevel);
            
            // Assert
            Assert.IsNotNull(_gameWorld.CurrentMap);
            Assert.AreEqual(2, _gameWorld.CurrentMap.Level);
            Assert.IsNotNull(_gameWorld.Player);
            Assert.AreEqual(_gameWorld.Player.CurrentMap, _gameWorld.CurrentMap);
        }
        
        [TestMethod]
        public void Should_Force_Move_Up()
        {
            // Arrange
            _gameWorld.NewGame();

            // Act
            _gameWorld.ForceLevelChange(ForceLevelChange.NextLevel);
            _gameWorld.ForceLevelChange(ForceLevelChange.NextLevel);
            _gameWorld.ForceLevelChange(ForceLevelChange.PreviousLevel);
            
            // Assert
            Assert.IsNotNull(_gameWorld.CurrentMap);
            Assert.AreEqual(2, _gameWorld.CurrentMap.Level);
            Assert.IsNotNull(_gameWorld.Player);
            Assert.AreEqual(_gameWorld.Player.CurrentMap, _gameWorld.CurrentMap);
        }
        
        [TestMethod]
        public void Should_Stay_On_Same_Map_When_Force_Move_Previous_On_Level_1()
        {
            // Arrange
            _gameWorld.NewGame();

            // Act
            _gameWorld.ForceLevelChange(ForceLevelChange.PreviousLevel);
            
            // Assert
            Assert.IsNotNull(_gameWorld.CurrentMap);
            Assert.AreEqual(1, _gameWorld.CurrentMap.Level);
            Assert.IsNotNull(_gameWorld.Player);
            Assert.AreEqual(_gameWorld.Player.CurrentMap, _gameWorld.CurrentMap);
        }
    }
}