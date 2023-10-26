using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class ReplayTests : BaseGameWorldIntegrationTests
    {
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
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
            SetupGameWorldWithCustomMapNoMonstersNoItemsNoExitsNoStructures(newGameWorld);

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
            SetupGameWorldWithCustomMapNoMonstersNoItemsNoExitsNoStructures(newGameWorld);

            newGameWorld.LoadReplay("TestReplay");
            newGameWorld.Player.Position = new Point(0, 0);

            // Act
            var result = newGameWorld.ExecuteNextReplayCommand();

            // Assert
            Assert.IsTrue(result.HasMoreCommands);
            Assert.AreNotSame(_gameWorld, newGameWorld);
            Assert.AreEqual(1, newGameWorld.HistoricalCommands.Count());
            Assert.AreEqual(1, newGameWorld.HistoricalCommands.WalkCommands.Count);
            Assert.AreEqual(new Point(0, 1), newGameWorld.Player.Position);

            Assert.AreEqual(2, result.CommandResults.Count);
            Assert.AreEqual(newGameWorld.HistoricalCommands.WalkCommands.First().CommandResult, result.CommandResults.First());
            Assert.AreEqual(newGameWorld.HistoricalCommands.WalkCommands.First().CommandResult.SubsequentCommands.First().CommandResult, result.CommandResults.Skip(1).First());
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
            SetupGameWorldWithCustomMapNoMonstersNoItemsNoExitsNoStructures(newGameWorld);

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
            SetupGameWorldWithCustomMapNoMonstersNoItemsNoExitsNoStructures(newGameWorld);

            newGameWorld.LoadReplay("TestReplay");

            newGameWorld.Player.Position = new Point(0, 0);

            // Act
            var result = newGameWorld.ExecuteNextReplayCommand();

            // Assert
            Assert.IsFalse(result.HasMoreCommands);
            Assert.AreNotSame(_gameWorld, newGameWorld);
            Assert.AreEqual(0, newGameWorld.HistoricalCommands.Count());
            Assert.AreEqual(0, newGameWorld.HistoricalCommands.WalkCommands.Count);
            Assert.AreEqual(new Point(0, 0), newGameWorld.Player.Position);
        }
        
        [TestMethod]
        public void Should_Replay_ApplyMachineCommand()
        {
            // TODO Failing on load.  May have something to do 
            // with replay creating the game world based on seed rather
            // than actually loading the game.
            
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
            _gameWorld.Player.Position = new Point(0, 2);

            var machineParams = new SpawnMachineParams()
                .WithMachineType(MachineType.Analyzer)
                .AtPosition(new Point(0, 3))
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMachine(machineParams);

            _gameWorld.MoveRequest(Direction.Down);

            _gameWorld.SaveGame("TestReplay", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            SetupGameWorldWithCustomMapNoMonstersNoItemsNoExitsNoStructures(newGameWorld);

            newGameWorld.LoadReplay("TestReplay");
            newGameWorld.Player.Position = new Point(0, 2);

            // Act
            var result = newGameWorld.ExecuteNextReplayCommand();
            
            // Assert
            Assert.IsTrue(result.HasMoreCommands);
            Assert.AreNotSame(_gameWorld, newGameWorld);
            Assert.AreEqual(2, newGameWorld.HistoricalCommands.Count());
            Assert.AreEqual(1, newGameWorld.HistoricalCommands.WalkCommands.Count);
            Assert.AreEqual(new Point(0, 3), newGameWorld.Player.Position);

            Assert.AreEqual(2, result.CommandResults.Count);
            var walkCommand = newGameWorld.HistoricalCommands.WalkCommands[0];
            var applyMachineCommand = newGameWorld.HistoricalCommands.ApplyMachineCommands[0];
        }
    }
}
