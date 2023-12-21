using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
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

            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));
        }

        [TestMethod]
        public void Should_LoadReplay_With_No_Historical_Commands()
        {
            // Arrange
            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.SaveGame("TestReplay", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            SetupGameWorldWithTestLevelGenerator(newGameWorld);

            newGameWorld.LoadReplay("TestReplay");
            newGameWorld.Player.Position = new Point(0, 0);

            // Assert
            Assert.AreNotSame(_gameWorld, newGameWorld);
            Assert.AreEqual(0, newGameWorld.CommandCollection.GetReplayCommands().Length);
        }

        [TestMethod]
        public void Should_Replay_One_Of_Two_Walk_Commands()
        {
            // Arrange
            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.SaveGame("TestReplay", true);

            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            SetupGameWorldWithTestLevelGenerator(newGameWorld);

            newGameWorld.LoadReplay("TestReplay");
            newGameWorld.Player.Position = new Point(0, 0);

            // Act
            var result = newGameWorld.ExecuteNextReplayCommand();

            // Assert
            Assert.IsTrue(result.HasMoreCommands);
            Assert.AreNotSame(_gameWorld, newGameWorld);
            Assert.AreEqual(1, newGameWorld.CommandCollection.GetReplayCommands().Length);
            
            var walkCommands = newGameWorld.CommandCollection.GetReplayCommands()
                .Cast<WalkCommand>()
                .ToList();
            
            Assert.AreEqual(1, walkCommands.Count);
            Assert.AreEqual(new Point(0, 1), newGameWorld.Player.Position);

            Assert.AreEqual(2, result.CommandResults.Count);
            Assert.AreEqual(walkCommands.First().CommandResult, result.CommandResults.First());
            Assert.AreEqual(walkCommands.First().CommandResult.SubsequentCommands.First().CommandResult, result.CommandResults.Skip(1).First());
        }

        [TestMethod]
        public void Should_Replay_Two_Walk_Commands()
        {
            // Arrange
            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.SaveGame("TestReplay", true);

            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            SetupGameWorldWithTestLevelGenerator(newGameWorld);

            newGameWorld.LoadReplay("TestReplay");
            newGameWorld.Player.Position = new Point(0, 0);

            // Act
            newGameWorld.ExecuteNextReplayCommand();
            newGameWorld.ExecuteNextReplayCommand();

            // Assert
            Assert.AreNotSame(_gameWorld, newGameWorld);
            Assert.AreEqual(2, newGameWorld.CommandCollection.GetReplayCommands().Length);
            
            var walkCommands = newGameWorld.CommandCollection.GetReplayCommands()
                .Cast<WalkCommand>()
                .ToList();
            
            Assert.AreEqual(2, walkCommands.Count);
            Assert.AreEqual(new Point(0, 2), newGameWorld.Player.Position);
        }

        [TestMethod]
        public void Should_Return_False_And_Do_Nothing_If_No_Commands_To_Execute()
        {
            // Arrange
            _gameWorld.SaveGame("TestReplay", true);

            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            SetupGameWorldWithTestLevelGenerator(newGameWorld);

            newGameWorld.LoadReplay("TestReplay");

            newGameWorld.Player.Position = new Point(0, 0);

            // Act
            var result = newGameWorld.ExecuteNextReplayCommand();

            // Assert
            Assert.IsFalse(result.HasMoreCommands);
            Assert.AreNotSame(_gameWorld, newGameWorld);
            Assert.AreEqual(0, newGameWorld.CommandCollection.GetReplayCommands().Length);
            Assert.AreEqual(new Point(0, 0), newGameWorld.Player.Position);
        }
        
        [TestMethod]
        public void Should_Replay_ApplyMachineCommand()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 2));

            var machineParams = new SpawnMachineParams()
                .WithMachineType(MachineType.Analyzer)
                .AtPosition(new Point(0, 3))
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMachine(machineParams);
            _gameWorld.MoveRequest(Direction.Down);
            
            _gameWorld.SaveGame("TestReplay", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();

            // Replays create a new game and only load in the historical commands where
            // Command.PersistForReplay = true. This means we need to re-spawn the monster
            // with the same ID after LoadReplay.
            SetupGameWorldWithTestLevelGenerator(newGameWorld);
            newGameWorld.LoadReplay("TestReplay");
            
            newGameWorld.Player.Position = new Point(0, 2);

            var newMachineParams = new SpawnMachineParams()
                .WithMachineType(MachineType.Analyzer)
                .AtPosition(new Point(0, 3))
                .OnMap(newGameWorld.CurrentMap.Id);
            
            newGameWorld.SpawnMachine(newMachineParams);

            // Make sure the ID's are the same
            Assert.AreEqual(machineParams.Result.ID, newMachineParams.Result.ID);
            
            // Act
            var result = newGameWorld.ExecuteNextReplayCommand();
            
            // Assert
            Assert.IsTrue(result.HasMoreCommands);
            Assert.AreNotSame(_gameWorld, newGameWorld);
            
            Assert.AreEqual(1, newGameWorld.CommandCollection.GetReplayCommands().Length);

            var walkCommands = newGameWorld.CommandCollection.GetReplayCommands()
                .Cast<WalkCommand>()
                .ToList();
            
            Assert.AreEqual(1, walkCommands.Count);
            Assert.AreEqual(new Point(0, 2), newGameWorld.Player.Position);

            var walkCommandReplayResult = result.CommandResults[0].Command as WalkCommand; 
            Assert.IsNotNull(walkCommandReplayResult);
            
            var machineCommandReplayResult = result.CommandResults[0].Command.CommandResult.SubsequentCommands[0] as ApplyMachineCommand; 
            Assert.IsNotNull(machineCommandReplayResult);
            Assert.IsTrue(newMachineParams.Result.IsUsed);
        }

        [TestMethod]
        public void Should_Replay_ApplyMachineCommand_Then_Replay_Undo_Command()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 2));

            var machineParams = new SpawnMachineParams()
                .WithMachineType(MachineType.Analyzer)
                .AtPosition(new Point(0, 3))
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMachine(machineParams);
            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.CancelIdentify();
            
            _gameWorld.SaveGame("TestReplay", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();

            // Replays create a new game and only load in the historical commands where
            // Command.PersistForReplay = true. This means we need to re-spawn the monster
            // with the same ID after LoadReplay.
            SetupGameWorldWithTestLevelGenerator(newGameWorld);
            newGameWorld.LoadReplay("TestReplay");
            
            newGameWorld.Player.Position = new Point(0, 2);

            var newMachineParams = new SpawnMachineParams()
                .WithMachineType(MachineType.Analyzer)
                .AtPosition(new Point(0, 3))
                .OnMap(newGameWorld.CurrentMap.Id);
            
            newGameWorld.SpawnMachine(newMachineParams);

            // Make sure the ID's are the same
            Assert.AreEqual(machineParams.Result.ID, newMachineParams.Result.ID);
            
            // Act
            newGameWorld.ExecuteNextReplayCommand();
            
            // This is the undo command
            var result = newGameWorld.ExecuteNextReplayCommand();
            
            // Assert
            Assert.IsTrue(result.HasMoreCommands);
            Assert.AreNotSame(_gameWorld, newGameWorld);

            var replayCommands = newGameWorld.CommandCollection.GetReplayCommands();
            
            Assert.AreEqual(2, replayCommands.Length);
            
            Assert.IsTrue(replayCommands[0] is WalkCommand);
            Assert.IsTrue(replayCommands[1] is UndoCommand);
            
            Assert.AreEqual(new Point(0, 2), newGameWorld.Player.Position);

            var undoCommandReplayResult = result.CommandResults[0].Command as UndoCommand; 
            Assert.IsNotNull(undoCommandReplayResult);
            
            Assert.IsFalse(newMachineParams.Result.IsUsed);
        }
        
        [TestMethod]
        public void Should_Replay_ApplyItem()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);

            var itemParams = new SpawnItemParams()
                .WithItemType(ItemType.ShieldGenerator)
                .IntoPlayerInventory();
            
            _gameWorld.SpawnItem(itemParams);
            
            var item = _gameWorld
                .GetInventoryItems()
                .OrderBy(i => i.Key)
                .ToList()
                .Single();

            var commandResult = _gameWorld.IdentifyItemRequest(item.Key);
            
            Assert.IsTrue(commandResult[0].Result == CommandResultEnum.Success);
            
            _gameWorld.SaveGame("TestReplay", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();

            // Replays create a new game and only load in the historical commands where
            // Command.PersistForReplay = true. This means we need to re-spawn the monster
            // with the same ID after LoadReplay.
            SetupGameWorldWithTestLevelGenerator(newGameWorld);
            newGameWorld.LoadReplay("TestReplay");
            
            var newItemParams = new SpawnItemParams()
                .WithItemType(ItemType.ShieldGenerator)
                .IntoPlayerInventory();
            
            newGameWorld.SpawnItem(newItemParams);

            // Act
            var result = newGameWorld.ExecuteNextReplayCommand();
            
            // Assert
            Assert.IsTrue(result.HasMoreCommands);
            Assert.AreNotSame(_gameWorld, newGameWorld);
            
            var replayCommands = newGameWorld.CommandCollection.GetReplayCommands();
            
            Assert.AreEqual(1, replayCommands.Length);

            Assert.IsTrue(replayCommands[0] is IdentifyItemCommand);

            Assert.AreEqual(1, result.CommandResults.Count);
            var identifyItemCommand = result.CommandResults[0].Command as IdentifyItemCommand;
            
            Assert.IsNotNull(identifyItemCommand);
            Assert.IsTrue(identifyItemCommand.CommandResult.Result == CommandResultEnum.Success);
        }
    }
}
