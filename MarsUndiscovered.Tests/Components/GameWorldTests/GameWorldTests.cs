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
            
            // We can't get exact expected map widths and heights as each map generator is different,
            // so just test that they are sensible values
            Assert.IsTrue(_gameWorld.CurrentMap.Width >= 10);
            Assert.IsTrue(_gameWorld.CurrentMap.Width < 200);
            Assert.IsTrue(_gameWorld.CurrentMap.Height >= 10);
            Assert.IsTrue(_gameWorld.CurrentMap.Height < 200);
        }
    }
}