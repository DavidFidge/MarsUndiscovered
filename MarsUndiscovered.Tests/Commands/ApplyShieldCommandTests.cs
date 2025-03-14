using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Tests.Components;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class ApplyShieldCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void ApplyShieldCommand_Should_Add_A_Shield_To_Player_Overwriting_Existing_Shield()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator).InInventory(_gameWorld.Inventory));
            var item = _gameWorld.Items.First().Value;

            _gameWorld.Player.Shield = 1;

            var commandFactory = Container.Resolve<ICommandCollection>();

            var applyShieldCommand = commandFactory.CreateCommand<ApplyShieldCommand>(_gameWorld);
            applyShieldCommand.Initialise(item, _gameWorld.Player);

            // Act
            var result = applyShieldCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            
            Assert.AreEqual((item.DamageShieldPercentage * _gameWorld.Player.MaxHealth) / 100, _gameWorld.Player.Shield);
            Assert.AreEqual("A soft glow and rhythmic hum surrounds me", result.Messages[0]);
            
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }
    }
}
