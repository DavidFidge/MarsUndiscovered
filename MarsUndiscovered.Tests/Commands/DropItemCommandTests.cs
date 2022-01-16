using System.Linq;
using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class DropItemCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void DropItemCommand_Should_Drop_Item_At_Players_Location()
        {
            // Arrange
            NewGameWithNoWallsNoMonstersNoItems();
            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(_gameWorld.Player.Position));
            var item = _gameWorld.Items.First().Value;
            _gameWorld.Inventory.Add(item);
            _gameWorld.Map.RemoveEntity(item);
            item.Position = Point.None;

            var commandFactory = Container.Resolve<ICommandFactory>();

            var dropItemCommand = commandFactory.CreateDropItemCommand(_gameWorld);
            dropItemCommand.Initialise(item, _gameWorld.Player);

            // Act
            var result = dropItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.AreSame(item, _gameWorld.Map.GetObjectAt<Item>(_gameWorld.Player.Position));
            Assert.AreEqual("You drop a Magnesium Pipe", result.Messages[0]);
        }

        [TestMethod]
        public void DropItemCommand_Should_Not_Drop_Item_If_Item_Exists_At_Players_Location()
        {
            // Arrange
            NewGameWithNoWallsNoMonstersNoItems();
            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(_gameWorld.Player.Position));
            var item1 = _gameWorld.Items.First().Value;
            _gameWorld.Inventory.Add(item1);
            _gameWorld.Map.RemoveEntity(item1);
            item1.Position = Point.None;
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(_gameWorld.Player.Position));
            var item2 = _gameWorld.Items.Skip(1).First().Value;

            var commandFactory = Container.Resolve<ICommandFactory>();

            var dropItemCommand = commandFactory.CreateDropItemCommand(_gameWorld);
            dropItemCommand.Initialise(item1, _gameWorld.Player);

            // Act
            var result = dropItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.AreNotSame(item1, _gameWorld.Map.GetObjectAt<Item>(_gameWorld.Player.Position));
            Assert.AreSame(item2, _gameWorld.Map.GetObjectAt<Item>(_gameWorld.Player.Position));
            Assert.AreEqual("Cannot drop item - there is another item in the way", result.Messages[0]);
        }
    }
}