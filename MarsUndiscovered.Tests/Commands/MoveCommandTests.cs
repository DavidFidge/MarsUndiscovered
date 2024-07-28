using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Tests.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class MoveCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void MoveCommand_Should_Move_GameObject()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            var commandFactory = Container.Resolve<ICommandCollection>();

            var moveCommand = commandFactory.CreateCommand<MoveCommand>(_gameWorld);
            var newPosition = new Point(0, 1);

            moveCommand.Initialise(_gameWorld.Player, new Tuple<Point, Point>(_gameWorld.Player.Position, newPosition));

            // Act
            var result = moveCommand.Execute();

            // Assert
            Assert.AreEqual(_gameWorld.Player.Position, newPosition);
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }

        [TestMethod]
        public void MoveCommand_Should_Create_Subsequent_Command_To_Pick_Up_Object()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 1));
            var newPosition = new Point(0, 2);

            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(newPosition));

            var commandFactory = Container.Resolve<ICommandCollection>();

            var moveCommand = commandFactory.CreateCommand<MoveCommand>(_gameWorld);

            moveCommand.Initialise(_gameWorld.Player, new Tuple<Point, Point>(_gameWorld.Player.Position, newPosition));

            // Act
            var result = moveCommand.Execute();

            // Assert
            Assert.AreEqual(1, result.SubsequentCommands.Count);
            Assert.IsTrue(result.SubsequentCommands.First() is PickUpItemCommand);
        }
    }
}
