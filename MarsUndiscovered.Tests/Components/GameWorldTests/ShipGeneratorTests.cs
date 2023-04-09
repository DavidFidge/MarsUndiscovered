using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class ShipGeneratorTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void ShipGenerator_Should_Create_Ship_Objects()
        {
            // Arrange
            var gameObjectFactory = Substitute.For<IGameObjectFactory>();
            var testGameWorld = Substitute.For<IGameWorld>();

            var wallCollection = new WallCollection(gameObjectFactory);
            var floorCollection = new FloorCollection(gameObjectFactory);

            testGameWorld.Walls.Returns(wallCollection);
            testGameWorld.Floors.Returns(floorCollection);
        
            var arrayView = new ArrayView<IGameObject>(70, 50);

            uint index = 1;
            
            arrayView.ApplyOverlay(_ => new Wall(testGameWorld, index));

            var wallsFloors = arrayView.ToArray();
            
            var map = MapGenerator.CreateMap(
                testGameWorld,
                wallsFloors.OfType<Wall>().ToList(),
                wallsFloors.OfType<Floor>().ToList(),
                70,
                50
                );

            var shipCollection = new ShipCollection(gameObjectFactory);

            gameObjectFactory.CreateShip().Returns(_ => new Ship(testGameWorld, index++));
            gameObjectFactory.CreateFloor().Returns(_ => new Floor(testGameWorld, index++));

            var shipGenerator = new ShipGenerator();
            
            // Act
            shipGenerator.CreateShip(gameObjectFactory, map, shipCollection);

            // Assert
            Assert.IsTrue(shipCollection.Count > 0);

            var switches = 0;
            IGameObject lastObject = null;

            for (var x = 0; x < map.Width; x++)
            {
                var currentObject = map.GetObjectAt(x, map.Height - 6 - Constants.ShipOffset);
                
                Assert.IsTrue(currentObject is Wall || currentObject is Floor);

                if (lastObject == null)
                {
                    lastObject = currentObject;
                }
                else if (lastObject.GetType() != currentObject.GetType())
                {
                    switches++;
                    lastObject = currentObject;
                }
            }
            
            Assert.IsTrue(switches <= 2);
            Assert.IsTrue(switches > 0);

            var typesFound = new HashSet<Type>();
            
            for (var x = 0; x < map.Width; x++)
            {
                var currentObject = map.GetObjectsAt(x, map.Height - Constants.ShipOffset - 1);
                foreach (var item in currentObject)
                {
                    typesFound.Add(item.GetType());
                }
            }
            
            Assert.AreEqual(3, typesFound.Count);
            Assert.IsTrue(typesFound.Contains(typeof(Ship)));
        }
   }
}