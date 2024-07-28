using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Tests.Components;
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
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            var spawnMonsterParams = new SpawnMonsterParams()
                .WithBreed("Roach")
                .AtPosition(new Point(0, 1))
                .WithState(MonsterState.Hunting);
            
            _gameWorld.SpawnMonster(spawnMonsterParams);
            var monster = _gameWorld.Monsters.Values.First();
            _gameWorld.Player.UnarmedAttack.DamageRange = new Range<int>(5, 5);
            var healthBefore = monster.Health;

            // Act
            _gameWorld.MoveRequest(Direction.Down);

            // Assert
            var walkCommand = _gameWorld.CommandCollection.GetCommands<WalkCommand>()[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.Count);

            var attackCommand =
                _gameWorld.CommandCollection.GetCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.First() as MeleeAttackCommand;
            Assert.IsNotNull(attackCommand);
            Assert.AreEqual(CommandResultEnum.Success, attackCommand.CommandResult.Result);

            Assert.AreEqual(healthBefore - 5, monster.Health);
            Assert.AreEqual("I hit the roach", attackCommand.CommandResult.Messages[0]);
            Assert.AreSame(_gameWorld.Player, attackCommand.Source);
            Assert.AreSame(monster, attackCommand.Target);
            Assert.AreEqual(MonsterState.Hunting, monster.MonsterState);
            
            Assert.IsFalse(attackCommand.EndsPlayerTurn);
            Assert.IsFalse(attackCommand.RequiresPlayerInput);
            Assert.IsTrue(attackCommand.InterruptsMovement);
        }
        
        [TestMethod]
        public void AttackCommand_Should_Deduct_Health_After_Shield()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 1)));
            var monster = _gameWorld.Monsters.Values.First();
            _gameWorld.Player.UnarmedAttack.DamageRange = new Range<int>(5, 5);
            var healthBefore = monster.Health;
            monster.Shield = 1;

            // Act
            _gameWorld.MoveRequest(Direction.Down);

            // Assert
            var walkCommand = _gameWorld.CommandCollection.GetCommands<WalkCommand>()[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.Count);

            var attackCommand =
                _gameWorld.CommandCollection.GetCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.First() as MeleeAttackCommand;
            Assert.IsNotNull(attackCommand);
            Assert.AreEqual(CommandResultEnum.Success, attackCommand.CommandResult.Result);

            Assert.AreEqual(healthBefore - 4, monster.Health);
            Assert.AreEqual(0, monster.Shield);
            Assert.AreEqual("I hit the roach", attackCommand.CommandResult.Messages[0]);
            Assert.AreSame(_gameWorld.Player, attackCommand.Source);
            Assert.AreSame(monster, attackCommand.Target);
        }
        
        [TestMethod]
        public void AttackCommand_Should_Deduct_Shield_Before_Health()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 1)));
            var monster = _gameWorld.Monsters.Values.First();
            _gameWorld.Player.UnarmedAttack.DamageRange = new Range<int>(5, 5);
            var healthBefore = monster.Health;
            monster.Shield = 6;

            // Act
            _gameWorld.MoveRequest(Direction.Down);

            // Assert
            var walkCommand = _gameWorld.CommandCollection.GetCommands<WalkCommand>()[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.Count);

            var attackCommand =
                _gameWorld.CommandCollection.GetCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.First() as MeleeAttackCommand;
            Assert.IsNotNull(attackCommand);
            Assert.AreEqual(CommandResultEnum.Success, attackCommand.CommandResult.Result);

            Assert.AreEqual(healthBefore, monster.Health);
            Assert.AreEqual(1, monster.Shield);
            Assert.AreEqual("I hit the roach", attackCommand.CommandResult.Messages[0]);
            Assert.AreSame(_gameWorld.Player, attackCommand.Source);
            Assert.AreSame(monster, attackCommand.Target);
        }
        
        [TestMethod]
        public void AttackCommand_Should_Kill_Monster_If_Health_Drops_Below_Zero()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 1)));
            var monster = _gameWorld.Monsters.Values.First();
            _gameWorld.Player.UnarmedAttack.DamageRange = new Range<int>(500000, 500000);
            var healthBefore = monster.Health;

            // Act
            _gameWorld.MoveRequest(Direction.Down);

            // Assert
            var walkCommand = _gameWorld.CommandCollection.GetCommands<WalkCommand>()[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.Count);

            var attackCommand =
                _gameWorld.CommandCollection.GetCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.First() as MeleeAttackCommand;
            Assert.IsNotNull(attackCommand);
            Assert.AreEqual(CommandResultEnum.Success, attackCommand.CommandResult.Result);

            Assert.AreEqual(healthBefore - 500000, monster.Health);
            Assert.IsTrue(monster.Health < 0);

            Assert.AreEqual(1, attackCommand.CommandResult.SubsequentCommands.Count);
            var deathCommand = attackCommand.CommandResult.SubsequentCommands.First() as DeathCommand;
            Assert.IsNotNull(deathCommand);
        }
        
        [TestMethod]
        public void AttackCommand_With_Concussive_Weapon_Should_Concuss()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            var spawnMonsterParams = new SpawnMonsterParams()
                .WithBreed("Roach")
                .AtPosition(new Point(0, 1))
                .WithState(MonsterState.Hunting);
            
            _gameWorld.SpawnMonster(spawnMonsterParams);
            var monster = _gameWorld.Monsters.Values.First();
            monster.Breed.WeaknessToConcuss = true;
            
            var item = SpawnItemAndAddToInventory(_gameWorld, ItemType.MagnesiumPipe);
            _gameWorld.Inventory.Equip(item);

            Assert.IsFalse(monster.IsConcussed);

            // Act
            _gameWorld.MoveRequest(Direction.Down);

            // Assert
            var walkCommand = _gameWorld.CommandCollection.GetCommands<WalkCommand>()[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.Count);

            var attackCommand =
                _gameWorld.CommandCollection.GetCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.First() as MeleeAttackCommand;
            Assert.IsNotNull(attackCommand);
            Assert.AreEqual(CommandResultEnum.Success, attackCommand.CommandResult.Result);
            Assert.AreSame(monster, attackCommand.Target);
            Assert.IsTrue(monster.IsConcussed);
        }
    }
}
