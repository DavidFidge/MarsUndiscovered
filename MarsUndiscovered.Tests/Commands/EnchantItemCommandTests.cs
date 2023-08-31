using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Tests.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands;

[TestClass]
public class EnchantItemCommandTests : BaseGameWorldIntegrationTests
{
    [TestMethod]
    public void EnchantItemCommand_Should_Enchant_Item()
    {
        // Arrange
        NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
        _gameWorld.Player.Position = new Point(0, 0);
        var enhancementBotParams = new SpawnItemParams().WithItemType(ItemType.EnhancementBots)
            .InInventory(_gameWorld.Inventory);
        _gameWorld.SpawnItem(enhancementBotParams);

        var enhancementBots = enhancementBotParams.Result;

        var magnesiumPipeParams =
            new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).InInventory(_gameWorld.Inventory);
        _gameWorld.SpawnItem(magnesiumPipeParams);

        var magnesiumPipe = magnesiumPipeParams.Result;

        var commandFactory = Container.Resolve<ICommandFactory>();

        var enchantItemCommand = commandFactory.CreateEnchantItemCommand(_gameWorld);
        enchantItemCommand.Initialise(enhancementBots, magnesiumPipe);

        var currentEnchant = magnesiumPipe.EnchantmentLevel;

        var currentMeleeAttack = magnesiumPipe.MeleeAttack.DamageRange;

        // Act
        var result = enchantItemCommand.Execute();

        // Assert
        Assert.AreEqual(CommandResultEnum.Success, result.Result);
        Assert.AreEqual(currentEnchant + 1, magnesiumPipe.EnchantmentLevel);
        Assert.IsTrue(currentMeleeAttack.Max < magnesiumPipe.MeleeAttack.DamageRange.Max);
        Assert.IsTrue(currentMeleeAttack.Min < magnesiumPipe.MeleeAttack.DamageRange.Min);
    }
}