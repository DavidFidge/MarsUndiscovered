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
    public class LineAttackCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void LineAttackCommand_Should_Deduct_Health_Of_Targets_Along_A_Path()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();

            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 1)));
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 2)));
            var monster1 = _gameWorld.Monsters.Values.First();
            var monster2 = _gameWorld.Monsters.Values.Skip(1).First();

            var monster1HealthBefore = monster1.Health;
            var monster2HealthBefore = monster2.Health;

            var item = SpawnItemAndAddToInventory(ItemType.IronSpike);
            item.LineAttack.DamageRange = new Range<int>(1, 1);
            _gameWorld.Inventory.Equip(item);

            // Act
            _gameWorld.MoveRequest(Direction.Down);

            // Assert
            var walkCommand = _gameWorld.HistoricalCommands.WalkCommands[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.Count);

            var lineAttackCommand =
                _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.First() as
                    LineAttackCommand;
            Assert.IsNotNull(lineAttackCommand);
            Assert.AreEqual(CommandResultEnum.Success, lineAttackCommand.CommandResult.Result);

            Assert.AreEqual(monster1HealthBefore - 1, monster1.Health);
            Assert.AreEqual(monster2HealthBefore - 1, monster2.Health);
        }

        [TestMethod]
        public void LineAttackCommand_Should_Deduct_Health_Of_Target_When_Target_Is_Two_Spaces_Away()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();

            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 2)));
            var monster = _gameWorld.Monsters.Values.First();

            var monsterHealthBefore = monster.Health;
            
            var item = SpawnItemAndAddToInventory(ItemType.IronSpike);
            item.LineAttack.DamageRange = new Range<int>(1, 1);
            _gameWorld.Inventory.Equip(item);
            
            // Act
            _gameWorld.MoveRequest(Direction.Down);

            // Assert
            var walkCommand = _gameWorld.HistoricalCommands.WalkCommands[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.Count);

            var lineAttackCommand =
                _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.First() as
                    LineAttackCommand;
            Assert.IsNotNull(lineAttackCommand);
            Assert.AreEqual(CommandResultEnum.Success, lineAttackCommand.CommandResult.Result);

            Assert.AreEqual(monsterHealthBefore - 1, monster.Health);
        }
    }
}