using Castle.MicroKernel.Registration;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class ReplayTests : BaseGameWorldIntegrationTests
    {
        private HalfWallsGenerator _halfWallsGenerator;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            // Currently some walls are needed for spawning turrets. Should be able to remove this in the future once proper
            // monster generation is implemented.
            _halfWallsGenerator = new HalfWallsGenerator(_gameWorld.GameObjectFactory, _gameWorld.MapGenerator);

            Container.Register(

                Component.For<IMapGenerator>()
                    .ImplementedBy<HalfWallsGenerator>()
                    .IsDefault()
            );

            NewGameWithCustomMap(_halfWallsGenerator);
        }

        [TestMethod]
        public void Should_LoadReplay_With_No_Historical_Commands()
        {
            // Arrange
            _gameWorld.Player.Position = new Point(0, 0);

            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.SaveGame("TestReplay", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();

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
            _gameWorld.Player.Position = new Point(0, 0);

            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.SaveGame("TestReplay", true);

            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();

            newGameWorld.LoadReplay("TestReplay");
            newGameWorld.Player.Position = new Point(0, 0);

            // Act
            var result = newGameWorld.ExecuteNextReplayCommand();

            // Assert
            Assert.IsTrue(result);
            Assert.AreNotSame(_gameWorld, newGameWorld);
            Assert.AreEqual(1, newGameWorld.HistoricalCommands.Count());
            Assert.AreEqual(1, newGameWorld.HistoricalCommands.WalkCommands.Count);
            Assert.AreEqual(new Point(0, 1), newGameWorld.Player.Position);
        }

        [TestMethod]
        public void Should_Replay_Two_Walk_Commands()
        {
            // Arrange
            _gameWorld.Player.Position = new Point(0, 0);

            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.SaveGame("TestReplay", true);

            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();

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
        public void Should_Return_False_And_Do_Nothing_If_No_Commands_To_Execute()
        {
            // Arrange
            _gameWorld.Player.Position = new Point(0, 0);

            _gameWorld.SaveGame("TestReplay", true);

            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();

            newGameWorld.LoadReplay("TestReplay");

            newGameWorld.Player.Position = new Point(0, 0);

            // Act
            var result = newGameWorld.ExecuteNextReplayCommand();

            // Assert
            Assert.IsFalse(result);
            Assert.AreNotSame(_gameWorld, newGameWorld);
            Assert.AreEqual(0, newGameWorld.HistoricalCommands.Count());
            Assert.AreEqual(0, newGameWorld.HistoricalCommands.WalkCommands.Count);
            Assert.AreEqual(new Point(0, 0), newGameWorld.Player.Position);
        }
    }
}
