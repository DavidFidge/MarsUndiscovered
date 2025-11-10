using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Tests.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class WaitCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void WaitCommand_With_Monster_Adjacent_Should_Result_In_Monster_Attacking_Player()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            var spawnMonsterParams = new SpawnMonsterParams()
                .WithBreed("Roach")
                .AtPosition(new Point(0, 1))
                .WithState(MonsterState.Hunting);
            
            _gameWorld.SpawnMonster(spawnMonsterParams);
            
            var monster = _gameWorld.Monsters.Values.First();
            var player = _gameWorld.Player;
            var healthBefore = player.Health;

            // Don't regenerate so that health is still deducted at end of turn
            player.RegenRate = 0;

            // Act
            var commandResults = _gameWorld.MoveRequest(Direction.None);

            // Assert
            Assert.AreEqual(new Point(0, 0), player.Position);
            Assert.AreEqual(new Point(0, 1), monster.Position);

            Assert.AreEqual(2, commandResults.Count);

            var waitCommandResult = commandResults[0];
            var attackCommandResult = commandResults[1];

            Assert.IsInstanceOfType(waitCommandResult.Command, typeof(WaitCommand));
            Assert.IsInstanceOfType(attackCommandResult.Command, typeof(MeleeAttackCommand));
            Assert.AreEqual("The roach hit me", attackCommandResult.Messages[0]);
            Assert.IsTrue(player.Health < healthBefore);
            
            Assert.IsTrue(waitCommandResult.Command.EndsPlayerTurn);
            Assert.IsFalse(waitCommandResult.Command.RequiresPlayerInput);
            Assert.IsFalse(waitCommandResult.Command.InterruptsMovement);
        }

        [TestMethod]
        public void Should_Wait()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            // Act
            var result = _gameWorld.MoveRequest(Direction.None);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result[0].Command is WaitCommand);
        }
    }
}
