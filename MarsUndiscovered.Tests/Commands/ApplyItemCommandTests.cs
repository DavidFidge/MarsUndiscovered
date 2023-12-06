using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Tests.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class ApplyItemCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void ApplyItemCommand_Should_Not_Apply_Other_Types_Of_Items()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
            _gameWorld.Player.Position = new Point(0, 0);

            var item = SpawnItemAndAddToInventory(_gameWorld, ItemType.MagnesiumPipe);

            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyItemCommand = commandFactory.CreateCommand<ApplyItemCommand>(_gameWorld);
            applyItemCommand.Initialise(_gameWorld.Player, item);

            // Act
            var result = applyItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.NoMove, result.Result);
            Assert.AreEqual("A Magnesium Pipe cannot be applied. Did you mean to equip it?", result.Messages[0]);
        }
        
        [TestMethod]
        public void ApplyItemCommand_Should_Apply_Gadget_And_Put_Gadget_On_Cooldown()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
            _gameWorld.Player.Position = new Point(0, 0);

            var item = SpawnItemAndAddToInventory(_gameWorld, ItemType.ShieldGenerator);
            _gameWorld.Inventory.ItemTypeDiscoveries[ItemType.ShieldGenerator].IsItemTypeDiscovered = true;

            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyItemCommand = commandFactory.CreateCommand<ApplyItemCommand>(_gameWorld);
            applyItemCommand.Initialise(_gameWorld.Player, item);

            // Act
            var result = applyItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.IsNotNull(_gameWorld.Inventory.Items.SingleOrDefault(i => i.Equals(item)));
            Assert.IsTrue(item.CurrentRechargeDelay > 0);
            Assert.AreEqual("You apply a Shield Generator Gadget", result.Messages[0]);

            var subsequentCommand = result.SubsequentCommands[0] as ApplyShieldCommand;
            Assert.IsNotNull(subsequentCommand);
            
            Assert.IsTrue(result.Command.PersistForReplay);
            Assert.IsTrue(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }
 
        [TestMethod]
        public void ApplyItemCommand_Should_Identify_Gadget_If_Used_When_Unidentified()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
            _gameWorld.Player.Position = new Point(0, 0);

            var item = SpawnItemAndAddToInventory(_gameWorld, ItemType.ShieldGenerator);

            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyItemCommand = commandFactory.CreateCommand<ApplyItemCommand>(_gameWorld);
            applyItemCommand.Initialise(_gameWorld.Player, item);

            // Act
            var result = applyItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.IsNotNull(_gameWorld.Inventory.Items.SingleOrDefault(i => i.Equals(item)));
            Assert.IsTrue(item.CurrentRechargeDelay > 0);
            Assert.AreEqual($"The {_gameWorld.Inventory.ItemTypeDiscoveries.GetUndiscoveredDescription(item)} is a Shield Generator Gadget!", result.Messages[0]);
            
            var subsequentCommand = result.SubsequentCommands[0] as ApplyShieldCommand;
            Assert.IsNotNull(subsequentCommand);
        }
        
        [TestMethod]
        public void ApplyItemCommand_Should_Not_Apply_Gadget_If_It_Is_Still_On_Cooldown()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
            _gameWorld.Player.Position = new Point(0, 0);

            var item = SpawnItemAndAddToInventory(_gameWorld, ItemType.ShieldGenerator);
            item.CurrentRechargeDelay = 1;
            _gameWorld.Inventory.ItemTypeDiscoveries[ItemType.ShieldGenerator].IsItemTypeDiscovered = true;
            
            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyItemCommand = commandFactory.CreateCommand<ApplyItemCommand>(_gameWorld);
            applyItemCommand.Initialise(_gameWorld.Player, item);

            // Act
            var result = applyItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.NoMove, result.Result);
            Assert.IsNotNull(_gameWorld.Inventory.Items.SingleOrDefault(i => i.Equals(item)));
            Assert.AreEqual(1, item.CurrentRechargeDelay);
            Assert.AreEqual("Cannot apply the Shield Generator Gadget (99% charged). It is still recharging.", result.Messages[0]);
        }
        
        [TestMethod]
        public void ApplyItemCommand_Should_Apply_NanoFlask_And_Consume_NanoFlask()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
            _gameWorld.Player.Position = new Point(0, 0);

            var item = SpawnItemAndAddToInventory(_gameWorld, ItemType.HealingBots);
            _gameWorld.Inventory.ItemTypeDiscoveries[ItemType.HealingBots].IsItemTypeDiscovered = true;

            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyItemCommand = commandFactory.CreateCommand<ApplyItemCommand>(_gameWorld);
            applyItemCommand.Initialise(_gameWorld.Player, item);

            // Act
            var result = applyItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.IsNull(_gameWorld.Inventory.Items.SingleOrDefault(i => i.Equals(item)));
            Assert.IsTrue(_gameWorld.CurrentMap.GetObjectsAt<Item>(_gameWorld.Player.Position).IsEmpty());
            Assert.AreEqual("You apply a NanoFlask of Healing Bots", result.Messages[0]);

            var subsequentCommand = result.SubsequentCommands[0] as ApplyHealingBotsCommand;
            Assert.IsNotNull(subsequentCommand);
        }
        
        [TestMethod]
        public void ApplyItemCommand_Should_Identify_NanoFlask_If_Consumed_When_Unidentified()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
            _gameWorld.Player.Position = new Point(0, 0);

            var item = SpawnItemAndAddToInventory(_gameWorld, ItemType.HealingBots);

            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyItemCommand = commandFactory.CreateCommand<ApplyItemCommand>(_gameWorld);
            applyItemCommand.Initialise(_gameWorld.Player, item);

            // Act
            var result = applyItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.IsNull(_gameWorld.Inventory.Items.SingleOrDefault(i => i.Equals(item)));
            Assert.IsTrue(_gameWorld.CurrentMap.GetObjectsAt<Item>(_gameWorld.Player.Position).IsEmpty());
            Assert.IsTrue(_gameWorld.Inventory.ItemTypeDiscoveries[ItemType.HealingBots].IsItemTypeDiscovered);
            Assert.AreEqual($"The {_gameWorld.Inventory.ItemTypeDiscoveries.GetUndiscoveredDescription(item)} is a NanoFlask of Healing Bots!", result.Messages[0]);
            
            var subsequentCommand = result.SubsequentCommands[0] as ApplyHealingBotsCommand;
            Assert.IsNotNull(subsequentCommand);
        }
                
        [TestMethod]
        public void ApplyItemCommand_Apply_EnhancementBots_Should_Require_Player_Input()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
            _gameWorld.Player.Position = new Point(0, 0);

            var item = SpawnItemAndAddToInventory(_gameWorld, ItemType.EnhancementBots);
            _gameWorld.Inventory.ItemTypeDiscoveries[ItemType.EnhancementBots].IsItemTypeDiscovered = true;

            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyItemCommand = commandFactory.CreateCommand<ApplyItemCommand>(_gameWorld);
            applyItemCommand.Initialise(_gameWorld.Player, item);

            // Act
            var result = applyItemCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            Assert.IsNull(_gameWorld.Inventory.Items.SingleOrDefault(i => i.Equals(item)));
            Assert.IsTrue(_gameWorld.CurrentMap.GetObjectsAt<Item>(_gameWorld.Player.Position).IsEmpty());
            Assert.AreEqual("You apply a NanoFlask of Enhancement Bots", result.Messages[0]);

            Assert.AreEqual(0, result.SubsequentCommands.Count);
            
            Assert.IsTrue(result.Command.PersistForReplay);
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsTrue(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }
    }
}
