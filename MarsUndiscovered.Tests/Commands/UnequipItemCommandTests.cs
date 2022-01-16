using System.Linq;
using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class UnequipItemCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void UnequipItemCommand_Should_Unequip_Weapon()
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

            var equipItemCommand = commandFactory.CreateEquipItemCommand(_gameWorld);
            equipItemCommand.Initialise(item);
            equipItemCommand.Execute();

            var unequipItemCommand = commandFactory.CreateUnequipItemCommand(_gameWorld);
            unequipItemCommand.Initialise(item);

            // Act
            var result = unequipItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.IsNull(_gameWorld.Inventory.EquippedWeapon);
            Assert.AreEqual("You sheathe a Magnesium Pipe", result.Messages[0]);
        }

        [TestMethod]
        public void UnequipItemCommand_For_Item_Not_Equipped_Should_Do_Nothing()
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

            var unequipItemCommand = commandFactory.CreateUnequipItemCommand(_gameWorld);
            unequipItemCommand.Initialise(item);

            // Act
            var result = unequipItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.IsNull(_gameWorld.Inventory.EquippedWeapon);
            Assert.AreEqual($"Item is already unequipped", result.Messages[0]);
        }
    }
}