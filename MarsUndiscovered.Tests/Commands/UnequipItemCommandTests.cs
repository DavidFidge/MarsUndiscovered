using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
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
            NewGameWithTestLevelGenerator(_gameWorld);
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(_gameWorld.Player.Position));
            var item = _gameWorld.Items.First().Value;
            _gameWorld.Inventory.Add(item);
            _gameWorld.CurrentMap.RemoveEntity(item);
            item.Position = Point.None;

            var commandFactory = Container.Resolve<ICommandCollection>();

            var equipItemCommand = commandFactory.CreateCommand<EquipItemCommand>(_gameWorld);
            equipItemCommand.Initialise(item);
            equipItemCommand.Execute();

            var unequipItemCommand = commandFactory.CreateCommand<UnequipItemCommand>(_gameWorld);
            unequipItemCommand.Initialise(item);

            // Act
            var result = unequipItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.IsNull(_gameWorld.Inventory.EquippedWeapon);
            Assert.AreEqual("I sheathe a Magnesium Pipe", result.Messages[0]);
            Assert.AreNotEqual(item.MeleeAttack.DamageRange.Min, _gameWorld.Player.MeleeAttack.DamageRange.Min);
            Assert.AreNotEqual(item.MeleeAttack.DamageRange.Max, _gameWorld.Player.MeleeAttack.DamageRange.Max);
            
            Assert.IsTrue(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }

        [TestMethod]
        public void UnequipItemCommand_For_Item_Not_Equipped_Should_Do_Nothing()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(_gameWorld.Player.Position));
            var item = _gameWorld.Items.First().Value;
            _gameWorld.Inventory.Add(item);
            _gameWorld.CurrentMap.RemoveEntity(item);
            item.Position = Point.None;

            var commandFactory = Container.Resolve<ICommandCollection>();

            var unequipItemCommand = commandFactory.CreateCommand<UnequipItemCommand>(_gameWorld);
            unequipItemCommand.Initialise(item);

            // Act
            var result = unequipItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.NoMove, result.Result);
            Assert.IsNull(_gameWorld.Inventory.EquippedWeapon);
            Assert.AreEqual("Item is already unequipped", result.Messages[0]);
        }
    }
}
