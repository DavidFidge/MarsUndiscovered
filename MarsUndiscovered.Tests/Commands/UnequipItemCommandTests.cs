using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Tests.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class UnequipItemCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void UnequipItemCommand_Should_Unequip_Weapon()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems(_gameWorld);
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

            var unequipItemCommand = commandFactory.CreateUnequipItemCommand(_gameWorld);
            unequipItemCommand.Initialise(item);

            // Act
            var result = unequipItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.IsNull(_gameWorld.Inventory.EquippedWeapon);
            Assert.AreEqual("You sheathe a Magnesium Pipe", result.Messages[0]);
            Assert.AreNotEqual(item.MeleeAttack.DamageRange.Min, _gameWorld.Player.MeleeAttack.DamageRange.Min);
            Assert.AreNotEqual(item.MeleeAttack.DamageRange.Max, _gameWorld.Player.MeleeAttack.DamageRange.Max);
        }

        [TestMethod]
        public void UnequipItemCommand_For_Item_Not_Equipped_Should_Do_Nothing()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems(_gameWorld);
            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(_gameWorld.Player.Position));
            var item = _gameWorld.Items.First().Value;
            _gameWorld.Inventory.Add(item);
            _gameWorld.CurrentMap.RemoveEntity(item);
            item.Position = Point.None;

            var commandFactory = Container.Resolve<ICommandFactory>();

            var unequipItemCommand = commandFactory.CreateUnequipItemCommand(_gameWorld);
            unequipItemCommand.Initialise(item);

            // Act
            var result = unequipItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.NoMove, result.Result);
            Assert.IsNull(_gameWorld.Inventory.EquippedWeapon);
            Assert.AreEqual($"Item is already unequipped", result.Messages[0]);
        }
    }
}
