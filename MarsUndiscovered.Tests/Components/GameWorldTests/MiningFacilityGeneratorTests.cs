using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Interfaces;
using NSubstitute;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class MiningFacilityGeneratorTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void MiningFacilityGenerator_Should_Create_MiningFacility_Objects()
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
            
            var map = MapGenerator.CreateMap(testGameWorld, 70, 50)
                .WithTerrain(wallsFloors.OfType<Wall>().ToList(), wallsFloors.OfType<Floor>().ToList());

            var miningFacilityCollection = new MiningFacilityCollection(gameObjectFactory);

            gameObjectFactory.CreateGameObject<MiningFacility>().Returns(_ => new MiningFacility(testGameWorld, index++));
            gameObjectFactory.CreateGameObject<Floor>().Returns(_ => new Floor(testGameWorld, index++));

            var miningFacilityGenerator = new MiningFacilityGenerator();
            
            // Act
            miningFacilityGenerator.CreateMiningFacility(gameObjectFactory, map, miningFacilityCollection);

            // Assert
            Assert.IsTrue(miningFacilityCollection.Count > 0);

            var switches = 0;
            IGameObject lastObject = null;

            for (var x = 0; x < map.Width; x++)
            {
                var currentObject = map.GetObjectAt(x, 0);
                
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
                var currentObject = map.GetObjectsAt(x, 1);
                foreach (var item in currentObject)
                {
                    typesFound.Add(item.GetType());
                }
            }
            
            Assert.AreEqual(3, typesFound.Count);
            Assert.IsTrue(typesFound.Contains(typeof(MiningFacility)));
        }
   }
}