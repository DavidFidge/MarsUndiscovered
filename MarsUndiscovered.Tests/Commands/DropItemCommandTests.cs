using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Tests.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class DropItemCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void DropItemCommand_Should_Drop_Item_At_Players_Location()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(_gameWorld.Player.Position));
            var item = _gameWorld.Items.First().Value;
            _gameWorld.Inventory.Add(item);
            _gameWorld.CurrentMap.RemoveEntity(item);
            item.Position = Point.None;

            var commandFactory = Container.Resolve<ICommandCollection>();

            var dropItemCommand = commandFactory.CreateCommand<DropItemCommand>(_gameWorld);
            dropItemCommand.Initialise(_gameWorld.Player, item);

            // Act
            var result = dropItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.AreSame(item, _gameWorld.CurrentMap.GetObjectAt<Item>(_gameWorld.Player.Position));
            Assert.AreEqual("You drop a Magnesium Pipe", result.Messages[0]);
            
            Assert.IsTrue(result.Command.PersistForReplay);
            Assert.IsTrue(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }

        [TestMethod]
        public void DropItemCommand_Should_Not_Drop_Item_If_Item_Exists_At_Players_Location()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));
            var spawnItemParams = new SpawnItemParams()
                .WithItemType(ItemType.MagnesiumPipe)
                .IntoPlayerInventory();
            
            _gameWorld.SpawnItem(spawnItemParams);
            var itemInInventory = spawnItemParams.Result;

            spawnItemParams = new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe);
            _gameWorld.SpawnItem(spawnItemParams);
            var item2 = spawnItemParams.Result;

            _gameWorld.Player.Position = item2.Position;

            var commandFactory = Container.Resolve<ICommandCollection>();

            var dropItemCommand = commandFactory.CreateCommand<DropItemCommand>(_gameWorld);
            dropItemCommand.Initialise(_gameWorld.Player, itemInInventory);

            // Act
            var result = dropItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.NoMove, result.Result);
            Assert.AreNotSame(itemInInventory, _gameWorld.CurrentMap.GetObjectAt<Item>(_gameWorld.Player.Position));
            Assert.AreSame(item2, _gameWorld.CurrentMap.GetObjectAt<Item>(_gameWorld.Player.Position));
            Assert.AreEqual("Cannot drop item - there is another item in the way", result.Messages[0]);
        }
    }
}
