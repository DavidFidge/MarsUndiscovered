using MarsUndiscovered.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}