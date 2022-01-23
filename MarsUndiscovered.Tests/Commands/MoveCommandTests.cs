using System;
using System.Linq;

using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Commands;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Tests.Components;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.Player.Position = new Point(0, 0);

            var commandFactory = Container.Resolve<ICommandFactory>();

            var moveCommand = commandFactory.CreateMoveCommand(_gameWorld);
            var newPosition = new Point(0, 1);

            moveCommand.Initialise(_gameWorld.Player, new Tuple<Point, Point>(_gameWorld.Player.Position, newPosition));

            // Act
            var result = moveCommand.Execute();

            // Assert
            Assert.AreEqual(_gameWorld.Player.Position, newPosition);
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
        }

        [TestMethod]
        public void MoveCommand_Should_Create_Subsequent_Command_To_Pick_Up_Object()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            var newPosition = new Point(0, 1);

            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(newPosition));
            _gameWorld.Player.Position = new Point(0, 0);

            var commandFactory = Container.Resolve<ICommandFactory>();

            var moveCommand = commandFactory.CreateMoveCommand(_gameWorld);

            moveCommand.Initialise(_gameWorld.Player, new Tuple<Point, Point>(_gameWorld.Player.Position, newPosition));

            // Act
            var result = moveCommand.Execute();

            // Assert
            Assert.AreEqual(1, result.SubsequentCommands.Count);
            Assert.IsTrue(result.SubsequentCommands.First() is PickUpItemCommand);
        }
    }
}