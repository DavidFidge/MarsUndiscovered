using System.Linq;
using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class EquipItemCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void EquipItemCommand_Should_Equip_Weapon()
        {
            // Arrange
            NewGameWithNoWallsNoMonstersNoItems();
            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(_gameWorld.Player.Position));
            var item = _gameWorld.Items.First().Value;
            _gameWorld.Inventory.Add(item);
            _gameWorld.CurrentMap.RemoveEntity(item);
            item.Position = Point.None;

            var commandFactory = Container.Resolve<ICommandFactory>();

            var equipItemCommand = commandFactory.CreateEquipItemCommand(_gameWorld);
            equipItemCommand.Initialise(item);

            // Act
            var result = equipItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.AreSame(item, _gameWorld.Inventory.EquippedWeapon);
            Assert.AreEqual("You wield a Magnesium Pipe", result.Messages[0]);
        }

        [TestMethod]
        public void EquipItemCommand_Should_Not_Equip_An_ItemType_That_Cannot_Be_Equipped()
        {
            // Arrange
            NewGameWithNoWallsNoMonstersNoItems();
            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots).AtPosition(_gameWorld.Player.Position));
            var item = _gameWorld.Items.First().Value;
            _gameWorld.Inventory.Add(item);
            _gameWorld.CurrentMap.RemoveEntity(item);
            item.Position = Point.None;

            var commandFactory = Container.Resolve<ICommandFactory>();

            var equipItemCommand = commandFactory.CreateEquipItemCommand(_gameWorld);
            equipItemCommand.Initialise(item);

            // Act
            var result = equipItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.NoMove, result.Result);
            Assert.AreEqual($"Cannot equip this type of item", result.Messages[0]);
        }

        [TestMethod]
        public void EquipItemCommand_Should_Swap_Weapon_If_One_Already_Equipped()
        {
            // Arrange
            NewGameWithNoWallsNoMonstersNoItems();
            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnItem(
                new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(_gameWorld.Player.Position)
            );
            var item1 = _gameWorld.Items.First().Value;
            _gameWorld.Inventory.Add(item1);
            _gameWorld.CurrentMap.RemoveEntity(item1);
            item1.Position = Point.None;

            _gameWorld.SpawnItem(
                new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(_gameWorld.Player.Position)
            );
            var item2 = _gameWorld.Items.Skip(1).First().Value;
            _gameWorld.Inventory.Add(item2);
            _gameWorld.CurrentMap.RemoveEntity(item2);
            item2.Position = Point.None;

            var commandFactory = Container.Resolve<ICommandFactory>();

            var equipItemCommand = commandFactory.CreateEquipItemCommand(_gameWorld);
            equipItemCommand.Initialise(item1);
            equipItemCommand.Execute();

            var equipItemCommand2 = commandFactory.CreateEquipItemCommand(_gameWorld);
            equipItemCommand2.Initialise(item2);

            // Act
            var result = equipItemCommand2.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.AreSame(item2, _gameWorld.Inventory.EquippedWeapon);
            Assert.IsTrue(_gameWorld.Inventory.Items.Contains(item1));
            Assert.IsTrue(_gameWorld.Inventory.Items.Contains(item2));
            Assert.AreEqual("You wield a Magnesium Pipe; you sheathe a Magnesium Pipe", result.Messages[0]);
        }

        [TestMethod]
        public void EquipItemCommand_Should_Do_Nothing_If_Trying_To_Equip_Item_Already_Equipped()
        {
            // Arrange
            NewGameWithNoWallsNoMonstersNoItems();
            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(_gameWorld.Player.Position));
            var item = _gameWorld.Items.First().Value;
            _gameWorld.Inventory.Add(item);
            _gameWorld.CurrentMap.RemoveEntity(item);
            item.Position = Point.None;

            var commandFactory = Container.Resolve<ICommandFactory>();

            var equipItemCommand = commandFactory.CreateEquipItemCommand(_gameWorld);
            equipItemCommand.Initialise(item);
            equipItemCommand.Execute();

            var equipItemCommand2 = commandFactory.CreateEquipItemCommand(_gameWorld);
            equipItemCommand2.Initialise(item);

            // Act
            var result = equipItemCommand2.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.NoMove, result.Result);
            Assert.AreSame(item, _gameWorld.Inventory.EquippedWeapon);
            Assert.IsTrue(_gameWorld.Inventory.Items.Contains(item));
            Assert.AreEqual("Item is already equipped", result.Messages[0]);
        }
    }
}