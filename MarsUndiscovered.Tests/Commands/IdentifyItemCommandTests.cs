using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Tests.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands;

[TestClass]
public class IdentifyItemCommandTests : BaseGameWorldIntegrationTests
{
    [TestMethod]
    public void IdentifyItemCommand_Should_Identify_Item()
    {
        // Arrange
        NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
        _gameWorld.Player.Position = new Point(0, 0);
        
        var magnesiumPipeParams =
            new SpawnItemParams()
                .WithItemType(ItemType.MagnesiumPipe)
                .WithEnchantmentLevel(2)
                .InInventory(_gameWorld.Inventory);
        
        _gameWorld.SpawnItem(magnesiumPipeParams);

        var magnesiumPipe = magnesiumPipeParams.Result;

        var commandFactory = Container.Resolve<ICommandCollection>();

        var identifyItemCommand = commandFactory.CreateCommand<IdentifyItemCommand>(_gameWorld);
        identifyItemCommand.Initialise(magnesiumPipe);

        // Act
        var result = identifyItemCommand.Execute();

        // Assert
        Assert.AreEqual(CommandResultEnum.Success, result.Result);
        Assert.IsTrue(magnesiumPipe.ItemDiscovery.IsEnchantLevelDiscovered);
        Assert.IsTrue(magnesiumPipe.ItemDiscovery.IsItemSpecialDiscovered);
        
        Assert.IsTrue(result.Command.PersistForReplay);
        Assert.IsTrue(result.Command.EndsPlayerTurn);
        Assert.IsFalse(result.Command.RequiresPlayerInput);
        Assert.IsFalse(result.Command.InterruptsMovement);
    }
    
    [TestMethod]
    public void IdentifyItemCommand_Should_Fail_If_Item_Is_Identified()
    {
        // Arrange
        NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
        _gameWorld.Player.Position = new Point(0, 0);
        
        var magnesiumPipeParams =
            new SpawnItemParams()
                .WithItemType(ItemType.MagnesiumPipe)
                .WithEnchantmentLevel(2)
                .InInventory(_gameWorld.Inventory);
        
        _gameWorld.SpawnItem(magnesiumPipeParams);

        var magnesiumPipe = magnesiumPipeParams.Result;

        magnesiumPipe.ItemDiscovery.IsItemSpecialDiscovered = true;
        magnesiumPipe.ItemDiscovery.IsEnchantLevelDiscovered = true;
        
        var commandFactory = Container.Resolve<ICommandCollection>();

        var identifyItemCommand = commandFactory.CreateCommand<IdentifyItemCommand>(_gameWorld);
        identifyItemCommand.Initialise(magnesiumPipe);

        // Act
        var result = identifyItemCommand.Execute();

        // Assert
        Assert.AreEqual(CommandResultEnum.NoMove, result.Result);
        Assert.AreEqual("The +2 Magnesium Pipe is already identified.", result.Messages[0]);
        Assert.IsTrue(magnesiumPipe.ItemDiscovery.IsEnchantLevelDiscovered);
        Assert.IsTrue(magnesiumPipe.ItemDiscovery.IsItemSpecialDiscovered);
        
        Assert.IsFalse(result.Command.PersistForReplay);
        Assert.IsFalse(result.Command.EndsPlayerTurn);
        Assert.IsFalse(result.Command.RequiresPlayerInput);
        Assert.IsFalse(result.Command.InterruptsMovement);
    }
}