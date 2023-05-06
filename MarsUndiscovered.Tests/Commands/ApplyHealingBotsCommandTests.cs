using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Tests.Components;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class ApplyHealingBotsCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void ApplyHealingBotsCommand_Should_Heal_Player_And_Increase_Max_Health()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems(_gameWorld);
            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots).InInventory(_gameWorld.Inventory));
            var item = _gameWorld.Items.First().Value;
            _gameWorld.Player.Health = 1;
            var currentMaxHealth = _gameWorld.Player.MaxHealth;

            var commandFactory = Container.Resolve<ICommandFactory>();

            var applyHealingBotsCommand = commandFactory.CreateApplyHealingBotsCommand(_gameWorld);
            applyHealingBotsCommand.Initialise(item, _gameWorld.Player);

            // Act
            var result = applyHealingBotsCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            
            Assert.AreEqual(currentMaxHealth + item.MaxHealthIncrease, _gameWorld.Player.MaxHealth);
            Assert.AreEqual(_gameWorld.Player.MaxHealth, _gameWorld.Player.Health);
            Assert.AreEqual(100, item.HealPercentOfMax);
            Assert.AreEqual("You feel healthier. All your ailments are cured and your max health has increased.", result.Messages[0]);
        }
    }
}
