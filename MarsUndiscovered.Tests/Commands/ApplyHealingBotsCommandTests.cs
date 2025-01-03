using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Tests.Components;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class ApplyHealingBotsCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void ApplyHealingBotsCommand_Should_Heal_Player_And_Increase_Max_Health()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots).InInventory(_gameWorld.Inventory));
            var item = _gameWorld.Items.First().Value;
            _gameWorld.Player.Health = 1;
            var currentMaxHealth = _gameWorld.Player.MaxHealth;

            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyHealingBotsCommand = commandFactory.CreateCommand<ApplyHealingBotsCommand>(_gameWorld);
            applyHealingBotsCommand.Initialise(item, _gameWorld.Player);

            // Act
            var result = applyHealingBotsCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            
            Assert.AreEqual(currentMaxHealth + item.MaxHealthIncrease, _gameWorld.Player.MaxHealth);
            Assert.AreEqual(_gameWorld.Player.MaxHealth, _gameWorld.Player.Health);
            Assert.AreEqual(100, item.HealPercentOfMax);
            Assert.AreEqual("I feel healthier and all my ailments are cured.", result.Messages[0]);
            
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }
    }
}
