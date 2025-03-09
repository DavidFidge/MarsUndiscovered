using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Maps;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class MapExitGeneratorTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void CreateMapEdgeExits_To_North()
        {
            // Arrange
            var lines = new[]
            {
                "...",
                "...",
                "..."
            };

            var mapTemplate = new MapTemplate(lines, 0, 0);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(1, 1), mapHeight: 3, mapWidth: 3);

            var mapExitGenerator = new MapExitGenerator
            {
                Mediator = _gameWorld.Mediator,
                DateTimeProvider = _gameWorld.DateTimeProvider,
                Logger = _gameWorld.Logger
            };

            var map1 = _gameWorld.Maps.First();
            var map2 = _gameWorld.Maps.Skip(1).First();
            
            // Act
            mapExitGenerator.CreateMapEdgeExits(_gameWorld, MapExitType.MapExitNorth, map1);
            
            // Assert
            var mapExits = _gameWorld.MapExits.Select(m => m.Value).ToList();
            
            AssertMapExit(mapExits[0], MapExitType.MapExitNorth, new Point(0, 0), map1, null);
            AssertMapExit(mapExits[1], MapExitType.MapExitNorth, new Point(1, 0), map1, null);
            AssertMapExit(mapExits[2], MapExitType.MapExitNorth, new Point(2, 0), map1, null);
        }

        [TestMethod]
        public void CreateMapEdgeExits_To_North_For_2_Map()
        {
            // Arrange
            var lines = new[]
            {
                "...",
                "...",
                "..."
            };

            var mapTemplate = new MapTemplate(lines, 0, 0);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(1, 1), mapHeight: 3, mapWidth: 3);

            var mapExitGenerator = new MapExitGenerator
            {
                Mediator = _gameWorld.Mediator,
                DateTimeProvider = _gameWorld.DateTimeProvider,
                Logger = _gameWorld.Logger
            };

            var map1 = _gameWorld.Maps.First();
            var map2 = _gameWorld.Maps.Skip(1).First();
            
            // Act
            mapExitGenerator.CreateMapEdgeExits(_gameWorld, MapExitType.MapExitNorth, map1);
            mapExitGenerator.CreateMapEdgeExits(_gameWorld, MapExitType.MapExitSouth, map2, map1);
            
            // Assert
            var mapExits = _gameWorld.MapExits.Select(m => m.Value).ToList();
            
            AssertMapExit(mapExits[0], MapExitType.MapExitNorth, new Point(0, 0), map1, mapExits[3]);
            AssertMapExit(mapExits[1], MapExitType.MapExitNorth, new Point(1, 0), map1, mapExits[4]);
            AssertMapExit(mapExits[2], MapExitType.MapExitNorth, new Point(2, 0), map1, mapExits[5]);

            AssertMapExit(mapExits[3], MapExitType.MapExitSouth, new Point(0, 2), map2, mapExits[0]);
            AssertMapExit(mapExits[4], MapExitType.MapExitSouth, new Point(1, 2), map2, mapExits[1]);
            AssertMapExit(mapExits[5], MapExitType.MapExitSouth, new Point(2, 2), map2, mapExits[2]);
        }
        
        public void AssertMapExit(MapExit mapExit, MapExitType expectedMapExitType, Point expectedPoint, MarsMap expectedMap, MapExit expectedLinkExit)
        {
            Assert.AreEqual(expectedPoint, mapExit.Position);
            Assert.AreEqual(expectedMapExitType, mapExit.MapExitType);
            Assert.AreEqual(expectedMap, mapExit.CurrentMap);
            Assert.AreEqual(expectedLinkExit, mapExit.Destination);
        }
    }
}
