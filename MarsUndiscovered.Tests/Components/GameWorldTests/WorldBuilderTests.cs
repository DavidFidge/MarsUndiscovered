using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Maps;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class WorldBuilderTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void ProgressiveWorldGeneration_Should_Perform_First_Step_Of_Map_Creation_And_Leave_Map_Unpopulated()
        {
            // Arrange
            var mapGenerator = new HalfWallsToBlankMapGenerator(_gameWorld.GameObjectFactory, Container.Resolve<IMapGenerator>());

            // Act
            ProgressiveWorldGenerationWithCustomMap(mapGenerator);

            // Assert
            Assert.AreEqual(1, mapGenerator.Steps);
            Assert.IsFalse(mapGenerator.IsComplete);
            Assert.AreSame(_gameWorld.CurrentMap, mapGenerator.MarsMap);
            
            Assert.IsNotNull(_gameWorld.CurrentMap);
            Assert.IsNull(_gameWorld.Player);
            Assert.IsNotNull(_gameWorld.GameObjects);
            Assert.IsTrue(_gameWorld.GameObjects.Count > 0);
            Assert.IsTrue(_gameWorld.Walls.Count > 0); // First step of the map generator will have walls
            Assert.IsTrue(_gameWorld.Seed > 0);
            Assert.AreEqual(MarsMap.MapWidth, _gameWorld.CurrentMap.Width);
            Assert.AreEqual(MarsMap.MapHeight, _gameWorld.CurrentMap.Height);
        }
        
        [TestMethod]
        public void ProgressiveWorldGeneration_Should_Perform_Second_Step_Of_Map_Creation_And_Complete_Map_But_Leave_It_Unpopulated()
        {
            // Arrange
            var mapGenerator = new HalfWallsToBlankMapGenerator(_gameWorld.GameObjectFactory, Container.Resolve<IMapGenerator>());

            ProgressiveWorldGenerationWithCustomMap(mapGenerator);
            
            var seed = _gameWorld.Seed;

            // Act
            _gameWorld.ProgressiveWorldGeneration(seed, 2, new WorldGenerationTypeParams(MapType.Outdoor));

            // Assert
            Assert.AreEqual(2, mapGenerator.Steps);
            Assert.IsTrue(mapGenerator.IsComplete);
            Assert.AreSame(_gameWorld.CurrentMap, mapGenerator.MarsMap);

            Assert.IsNotNull(_gameWorld.CurrentMap);
            Assert.IsNull(_gameWorld.Player);
            Assert.IsNotNull(_gameWorld.GameObjects);
            Assert.IsTrue(_gameWorld.GameObjects.Count > 0);
            Assert.AreEqual(0, _gameWorld.Walls.Count); // No walls in completed map
            Assert.IsTrue(_gameWorld.Seed > 0);
            Assert.AreEqual(MarsMap.MapWidth, _gameWorld.CurrentMap.Width);
            Assert.AreEqual(MarsMap.MapHeight, _gameWorld.CurrentMap.Height);
        }
        
        [TestMethod]
        public void ProgressiveWorldGeneration_Should_Populate_Map()
        {
            // Arrange
            var mapGenerator = new HalfWallsToBlankMapGenerator(_gameWorld.GameObjectFactory, Container.Resolve<IMapGenerator>());

            ProgressiveWorldGenerationWithCustomMap(mapGenerator);
            
            var seed = _gameWorld.Seed;

            // Act
            _gameWorld.ProgressiveWorldGeneration(seed, Int32.MaxValue, new WorldGenerationTypeParams(MapType.Outdoor));

            // Assert
            Assert.AreEqual(2, mapGenerator.Steps);
            Assert.IsTrue(mapGenerator.IsComplete);
            Assert.AreSame(_gameWorld.CurrentMap, mapGenerator.MarsMap);

            Assert.IsNotNull(_gameWorld.CurrentMap);
            Assert.IsNotNull(_gameWorld.Player);
            Assert.IsNotNull(_gameWorld.GameObjects);
            Assert.IsTrue(_gameWorld.GameObjects.Count > 0);
            Assert.AreEqual(0, _gameWorld.Walls.Count); // No walls in completed map
            Assert.IsTrue(_gameWorld.Seed > 0);
            Assert.AreEqual(MarsMap.MapWidth, _gameWorld.CurrentMap.Width);
            Assert.AreEqual(MarsMap.MapHeight, _gameWorld.CurrentMap.Height);
        }
    }
}