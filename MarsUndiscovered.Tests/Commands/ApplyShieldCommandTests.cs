using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Tests.Components;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class ApplyShieldCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void ApplyShieldCommand_Should_Add_A_Shield_To_Player_Overwriting_Existing_Shield()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator).InInventory(_gameWorld.Inventory));
            var item = _gameWorld.Items.First().Value;

            _gameWorld.Player.Shield = 1;

            var commandFactory = Container.Resolve<ICommandFactory>();

            var applyShieldCommand = commandFactory.CreateApplyShieldCommand(_gameWorld);
            applyShieldCommand.Initialise(item, _gameWorld.Player);

            // Act
            var result = applyShieldCommand.Execute();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result.Result);
            
            Assert.AreEqual((item.DamageShieldPercentage * _gameWorld.Player.MaxHealth) / 100, _gameWorld.Player.Shield);
            Assert.AreEqual("A soft glow and rhythmic hum surrounds you", result.Messages[0]);
        }
    }
}
