using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Tests.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class ApplyForcePushCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void ApplyForcePushCommand_Should_Push_All_Actors_Away_From_Location()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);
            
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
            Assert.AreEqual("A soft glow and rhythmic hum surrounds you", result.Messages[0]);
            
            Assert.IsFalse(result.Command.PersistForReplay);
            Assert.IsFalse(result.Command.EndsPlayerTurn);
            Assert.IsFalse(result.Command.RequiresPlayerInput);
            Assert.IsFalse(result.Command.InterruptsMovement);
        }
    }
}
