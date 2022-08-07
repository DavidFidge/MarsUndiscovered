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
    public class WorldBuilderTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void Should_Create_New_WorldBuilder()
        {
            // Act
            NewWorldBuilderWithCustomMap();

            // Assert
            Assert.IsNotNull(_gameWorld.CurrentMap);
            Assert.IsNull(_gameWorld.Player);
            Assert.IsNotNull(_gameWorld.GameObjects);
            Assert.IsTrue(_gameWorld.GameObjects.Count > 0);
            Assert.IsTrue(_gameWorld.Seed > 0);
            Assert.AreEqual(MarsMap.MapWidth, _gameWorld.CurrentMap.Width);
            Assert.AreEqual(MarsMap.MapHeight, _gameWorld.CurrentMap.Height);
        }
        
        [TestMethod]
        public void Should_Create_New_WorldBuilder_With_Objects_Populated()
        {
            // Arrange
            NewWorldBuilderWithCustomMap();
            var seed = _gameWorld.Seed;

            // Act
            _gameWorld.ProgressiveWorldGeneration(seed, 2);

            // Assert
            Assert.IsNotNull(_gameWorld.CurrentMap);
            Assert.IsNotNull(_gameWorld.Player);
            Assert.IsNotNull(_gameWorld.GameObjects);
            Assert.IsTrue(_gameWorld.GameObjects.Count > 0);
            Assert.IsTrue(_gameWorld.Seed > 0);
            Assert.AreEqual(MarsMap.MapWidth, _gameWorld.CurrentMap.Width);
            Assert.AreEqual(MarsMap.MapHeight, _gameWorld.CurrentMap.Height);
        }
    }
}