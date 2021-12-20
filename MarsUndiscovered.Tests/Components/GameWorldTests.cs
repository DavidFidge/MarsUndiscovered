using System.Linq;

using FrigidRogue.TestInfrastructure;

using MarsUndiscovered.Components;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarsUndiscovered.Tests.ViewModels
{
    [TestClass]
    public class GameWorldTests : BaseTest
    {
        private GameWorld _gameWorld;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            _gameWorld = SetupBaseComponent(new GameWorld());
        }

        [TestMethod]
        public void Should_Generate_Map()
        {
            // Arrange
            _gameWorld.WallFactory = new TestFactory<Wall>();
            _gameWorld.FloorFactory = new TestFactory<Floor>();

            // Act
            _gameWorld.Generate();

            // Assert
            Assert.AreEqual(GameWorld.MapWidth, _gameWorld.Map.Width);
            Assert.AreEqual(GameWorld.MapHeight, _gameWorld.Map.Height);
        }
    }
}