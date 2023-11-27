using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Tests.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class UndoCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void UndoCommand_Should_Undo_ApplyMachineCommand()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
            _gameWorld.Player.Position = new Point(0, 0);

            var machineParams = new SpawnMachineParams()
                .WithMachineType(MachineType.Analyzer)
                .OnMap(_gameWorld.CurrentMap.Id);
            
            _gameWorld.SpawnMachine(machineParams);

            var machine = machineParams.Result;
            
            var commandFactory = Container.Resolve<ICommandFactory>();

            var applyMachineCommand = commandFactory.CreateApplyMachineCommand(_gameWorld);
            applyMachineCommand.Initialise(machine);
            var applyMachineCommandResult = applyMachineCommand.Execute();
            
            // Usually this gets done by the game world
            _gameWorld.ReplayCommands.AddCommand(applyMachineCommand);
            
            Assert.IsTrue(machine.IsUsed);

            var undoCommand = commandFactory.CreateUndoCommand(_gameWorld);
            undoCommand.Initialise(applyMachineCommand.Id);
            
            // Act
            var result = undoCommand.Execute();
            
            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            
            var command = result.Command as UndoCommand;
         
            Assert.IsFalse(machine.IsUsed);
            
            Assert.IsTrue(result.Command.PersistForReplay);
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }
    }
}
