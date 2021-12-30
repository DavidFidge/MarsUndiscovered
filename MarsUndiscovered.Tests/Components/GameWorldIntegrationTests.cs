using System.Linq;
using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarsUndiscovered.Tests.Components
{
    [TestClass]
    public class GameWorldIntegrationTests : BaseIntegrationTest
    {
        private GameWorld _gameWorld;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            _gameWorld = (GameWorld)Container.Resolve<IGameWorld>();
        }

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
            Assert.IsNotNull(_gameWorld.Map);
            Assert.IsNotNull(_gameWorld.Player);
            Assert.IsNotNull(_gameWorld.GameObjects);
            Assert.IsTrue(_gameWorld.GameObjects.Count > 0);
            Assert.IsTrue(_gameWorld.Seed > 0);
            Assert.AreEqual(GameWorld.MapWidth, _gameWorld.Map.Width);
            Assert.AreEqual(GameWorld.MapHeight, _gameWorld.Map.Height);
        }

        [TestMethod]
        public void Should_Spawn_Monster()
        {
            // Arrange
            _gameWorld.NewGame();
            var currentMonsterCount = _gameWorld.Monsters.Count;

            // Act
            _gameWorld.SpawnMonster(Breed.Roach.Name);

            // Assert
            Assert.AreEqual(currentMonsterCount + 1, _gameWorld.Monsters.Count);
        }

        [TestMethod]
        public void Should_Save_Then_Load_Game()
        {
            // Arrange
            _gameWorld.NewGame();
            _gameWorld.SpawnMonster(Breed.Roach);
            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            Assert.AreNotSame(_gameWorld, newGameWorld);

            Assert.AreEqual(_gameWorld.Seed, newGameWorld.Seed);
            Assert.AreEqual(_gameWorld.GameObjectFactory.LastId, newGameWorld.GameObjectFactory.LastId);

            Assert.AreEqual(_gameWorld.GameObjects.Count, newGameWorld.GameObjects.Count);
            Assert.AreEqual(_gameWorld.GameObjects.Values.OfType<Wall>().Count(), newGameWorld.GameObjects.Values.OfType<Wall>().Count());
            Assert.AreEqual(_gameWorld.GameObjects.Values.OfType<Floor>().Count(), newGameWorld.GameObjects.Values.OfType<Floor>().Count());
            Assert.AreEqual(_gameWorld.GameObjects.Values.OfType<Monster>().Count(), newGameWorld.GameObjects.Values.OfType<Monster>().Count());
            Assert.AreEqual(_gameWorld.GameObjects.Values.OfType<Player>().Count(), newGameWorld.GameObjects.Values.OfType<Player>().Count());

            Assert.AreEqual(_gameWorld.Player.ID, newGameWorld.Player.ID);
            Assert.AreEqual(_gameWorld.Player.Position, newGameWorld.Player.Position);
            Assert.AreEqual(_gameWorld.Player.IsWalkable, newGameWorld.Player.IsWalkable);
            Assert.AreEqual(_gameWorld.Player.IsTransparent, newGameWorld.Player.IsTransparent);
        }
    }
}