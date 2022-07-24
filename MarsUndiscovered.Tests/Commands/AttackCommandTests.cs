using System.Linq;

using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Commands;
using MarsUndiscovered.Components;
using MarsUndiscovered.Tests.Components;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonoGame.Extended;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class AttackCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void AttackCommand_Should_Deduct_Health_Of_Target()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();

            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 1)));
            var monster = _gameWorld.Monsters.Values.First();
            _gameWorld.Player.MeleeAttack.DamageRange = new Range<int>(5, 5);
            var healthBefore = monster.Health;

            // Act
            _gameWorld.MoveRequest(Direction.Down);

            // Assert
            var walkCommand = _gameWorld.HistoricalCommands.WalkCommands[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.Count);

            var attackCommand =
                _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.First() as MeleeAttackCommand;
            Assert.IsNotNull(attackCommand);
            Assert.AreEqual(CommandResultEnum.Success, attackCommand.CommandResult.Result);

            Assert.AreEqual(healthBefore - 5, monster.Health);
            Assert.AreEqual("You hit the roach", attackCommand.CommandResult.Messages[0]);
            Assert.AreSame(_gameWorld.Player, attackCommand.Source);
            Assert.AreSame(monster, attackCommand.Target);
       }
        
        [TestMethod]
        public void AttackCommand_Should_Kill_Monster_If_Health_Drops_Below_Zero()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();

            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 1)));
            var monster = _gameWorld.Monsters.Values.First();
            _gameWorld.Player.MeleeAttack.DamageRange = new Range<int>(500000, 500000);
            var healthBefore = monster.Health;

            // Act
            _gameWorld.MoveRequest(Direction.Down);

            // Assert
            var walkCommand = _gameWorld.HistoricalCommands.WalkCommands[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.Count);

            var attackCommand =
                _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.First() as MeleeAttackCommand;
            Assert.IsNotNull(attackCommand);
            Assert.AreEqual(CommandResultEnum.Success, attackCommand.CommandResult.Result);

            Assert.AreEqual(healthBefore - 500000, monster.Health);
            Assert.IsTrue(monster.Health < 0);

            Assert.AreEqual(1, attackCommand.CommandResult.SubsequentCommands.Count);
            var deathCommand = attackCommand.CommandResult.SubsequentCommands.First() as DeathCommand;
            Assert.IsNotNull(deathCommand);
        }
    }
}