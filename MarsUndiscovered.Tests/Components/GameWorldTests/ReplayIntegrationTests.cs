using System.Linq;
using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Commands;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class WalkIntegrationTests : BaseIntegrationTest
    {
        private GameWorld _gameWorld;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            _gameWorld = (GameWorld)Container.Resolve<IGameWorld>();
        }

        [TestMethod]
        public void Should_LoadReplay_With_No_Historical_Commands()
        {
            // Arrange
            var blankMapGeneration = new BlankMapGenerator(
                Container.Resolve<IGameObjectFactory>(),
                Container.Resolve<IMapGenerator>()
            );

            _gameWorld.MapGenerator = blankMapGeneration;

            _gameWorld.NewGame();
            _gameWorld.Player.Position = new Point(0, 0);

            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.SaveGame("TestReplay", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.MapGenerator = blankMapGeneration;

            newGameWorld.LoadReplay("TestReplay");
            newGameWorld.Player.Position = new Point(0, 0);

            // Assert
            Assert.AreNotSame(_gameWorld, newGameWorld);
            Assert.AreEqual(0, newGameWorld.HistoricalCommands.Count());
        }

        [TestMethod]
        public void Should_Replay_One_Walk_Command()
        {
            // Arrange
            var blankMapGeneration = new BlankMapGenerator(
                Container.Resolve<IGameObjectFactory>(),
                Container.Resolve<IMapGenerator>()
            );

            _gameWorld.MapGenerator = blankMapGeneration;

            _gameWorld.NewGame();
            _gameWorld.Player.Position = new Point(0, 0);

            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.SaveGame("TestReplay", true);

            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.MapGenerator = blankMapGeneration;

            newGameWorld.LoadReplay("TestReplay");
            newGameWorld.Player.Position = new Point(0, 0);

            // Act
            newGameWorld.ExecuteNextReplayCommand();

            // Assert
            Assert.AreNotSame(_gameWorld, newGameWorld);
            Assert.AreEqual(1, newGameWorld.HistoricalCommands.Count());
            Assert.AreEqual(1, newGameWorld.HistoricalCommands.WalkCommands.Count);
            Assert.AreEqual(new Point(0, 1), newGameWorld.Player.Position);
        }

        [TestMethod]
        public void Should_Replay_Two_Walk_Commands()
        {
            // Arrange
            var blankMapGeneration = new BlankMapGenerator(
                Container.Resolve<IGameObjectFactory>(),
                Container.Resolve<IMapGenerator>()
            );

            _gameWorld.MapGenerator = blankMapGeneration;

            _gameWorld.NewGame();
            _gameWorld.Player.Position = new Point(0, 0);

            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.SaveGame("TestReplay", true);

            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.MapGenerator = blankMapGeneration;

            newGameWorld.LoadReplay("TestReplay");
            newGameWorld.Player.Position = new Point(0, 0);

            // Act
            newGameWorld.ExecuteNextReplayCommand();
            newGameWorld.ExecuteNextReplayCommand();

            // Assert
            Assert.AreNotSame(_gameWorld, newGameWorld);
            Assert.AreEqual(2, newGameWorld.HistoricalCommands.Count());
            Assert.AreEqual(2, newGameWorld.HistoricalCommands.WalkCommands.Count);
            Assert.AreEqual(new Point(0, 2), newGameWorld.Player.Position);
        }

        [TestMethod]
        public void Should_Do_Nothing_If_No_Commands_To_Execute()
        {
            // Arrange
            var blankMapGeneration = new BlankMapGenerator(
                Container.Resolve<IGameObjectFactory>(),
                Container.Resolve<IMapGenerator>()
            );

            _gameWorld.MapGenerator = blankMapGeneration;

            _gameWorld.NewGame();
            _gameWorld.Player.Position = new Point(0, 0);

            _gameWorld.SaveGame("TestReplay", true);

            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.MapGenerator = blankMapGeneration;

            newGameWorld.LoadReplay("TestReplay");
            newGameWorld.Player.Position = new Point(0, 0);

            // Act
            newGameWorld.ExecuteNextReplayCommand();

            // Assert
            Assert.AreNotSame(_gameWorld, newGameWorld);
            Assert.AreEqual(0, newGameWorld.HistoricalCommands.Count());
            Assert.AreEqual(0, newGameWorld.HistoricalCommands.WalkCommands.Count);
            Assert.AreEqual(new Point(0, 0), newGameWorld.Player.Position);
        }
    }
}