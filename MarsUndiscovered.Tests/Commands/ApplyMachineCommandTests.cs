using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Tests.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class ApplyMachineCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void ApplyMachineCommand_Should_Apply_Machine()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);

            var machineParams = new SpawnMachineParams()
                .WithMachineType(MachineType.Analyzer)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMachine(machineParams);

            var machine = machineParams.Result;
            
            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyMachineCommand = commandFactory.CreateCommand<ApplyMachineCommand>(_gameWorld);
            applyMachineCommand.Initialise(machine);

            // Act
            var result = applyMachineCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            
            var command = result.Command as ApplyMachineCommand;
         
            Assert.IsTrue(command.Machine.IsUsed);
            
            // At the moment moving into a machine will automatically create the machine command so
            // we don't need to persist it for replay.
            Assert.IsFalse(result.Command.PersistForReplay);
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsTrue(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }
        
        [TestMethod]
        public void ApplyMachineCommand_Should_Do_Nothing_If_Machine_Is_Used()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);

            var machineParams = new SpawnMachineParams()
                .WithMachineType(MachineType.Analyzer)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMachine(machineParams);

            var machine = machineParams.Result;
            machine.IsUsed = true;
            
            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyMachineCommand = commandFactory.CreateCommand<ApplyMachineCommand>(_gameWorld);
            applyMachineCommand.Initialise(machine);

            // Act
            var result = applyMachineCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.NoMove, result.Result);
            
            var command = result.Command as ApplyMachineCommand;
         
            Assert.IsTrue(command.Machine.IsUsed);
            
            // At the moment moving into a machine will automatically create the machine command so
            // we don't need to persist it for replay.
            Assert.IsFalse(result.Command.PersistForReplay);
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }
    }
}
