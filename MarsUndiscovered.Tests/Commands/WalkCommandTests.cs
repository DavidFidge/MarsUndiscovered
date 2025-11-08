using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Tests.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Commands
{
    [TestClass]
    public class WalkCommandTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void WalkCommand_Should_Move_Player_For_Valid_Square()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            // Act
            _gameWorld.MoveRequest(Direction.Down);

            // Assert
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>().Count);
            Assert.AreEqual(new Point(0, 1), _gameWorld.Player.Position);

            var walkCommand = _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.Count);

            var moveCommand =
                _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.First() as MoveCommand;

            Assert.IsNotNull(moveCommand);
            Assert.AreEqual(CommandResultEnum.Success, moveCommand.CommandResult.Result);
            
            Assert.IsTrue(walkCommand.EndsPlayerTurn);
            Assert.IsFalse(walkCommand.RequiresPlayerInput);
            Assert.IsFalse(walkCommand.InterruptsMovement);
        }

        [TestMethod]
        public void WalkCommand_Should_Return_NoMove_Result_For_Wall()
        {
            // Arrange
            var wallPosition = new Point(1, 0);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 0));

            // Act
            _gameWorld.MoveRequest(Direction.Right);

            // Assert
            Assert.AreEqual(0, _gameWorld.CommandCollection.GetLastCommands<MoveCommand>().Count);
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>().Count);
            Assert.AreEqual(new Point(0, 0), _gameWorld.Player.Position);

            var walkCommand = _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0];

            Assert.AreEqual(CommandResultEnum.NoMove, walkCommand.CommandResult.Result);
            Assert.AreEqual(0, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.Count);
        }

        [TestMethod]
        public void WalkCommand_Player_Into_Monster_Should_Attack()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 1)));
            var monster = _gameWorld.Monsters.Values.First();
            var healthBefore = monster.Health;

            // Act
            _gameWorld.MoveRequest(Direction.Down);

            // Assert
            Assert.AreEqual(0, _gameWorld.CommandCollection.GetLastCommands<MoveCommand>().Count);
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>().Count);
            Assert.AreEqual(new Point(0, 0), _gameWorld.Player.Position);

            var walkCommand = _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.Count);

            var attackCommand =
                _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.First() as MeleeAttackCommand;

            Assert.IsNotNull(attackCommand);
            Assert.AreEqual(CommandResultEnum.Success, attackCommand.CommandResult.Result);

            Assert.IsTrue(monster.Health < healthBefore);
            Assert.AreEqual("I hit the roach", attackCommand.CommandResult.Messages[0]);
        }

        [TestMethod]
        public void WalkCommand_Player_Into_Monster_On_Wall_Should_Attack()
        {
            // Arrange
            var wallPosition = new Point(0, 1);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 0));

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("TeslaTurret").AtPosition(wallPosition));

            var monster = _gameWorld.Monsters.Values.First();
            var healthBefore = monster.Health;

            // Act
            _gameWorld.MoveRequest(Direction.Down);

            // Assert
            Assert.AreEqual(0, _gameWorld.CommandCollection.GetLastCommands<MoveCommand>().Count);
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>().Count);
            Assert.AreEqual(new Point(0, 0), _gameWorld.Player.Position);

            var walkCommand = _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.Count);

            var attackCommand =
                _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.First() as MeleeAttackCommand;

            Assert.IsNotNull(attackCommand);
            Assert.AreEqual(CommandResultEnum.Success, attackCommand.CommandResult.Result);

            Assert.IsTrue(monster.Health < healthBefore);
            Assert.AreEqual("I hit the tesla turret", attackCommand.CommandResult.Messages[0]);
        }

        [TestMethod]
        public void Should_Walk_To_Position()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            var path = _gameWorld.GetPathToPlayer(new Point(2, 2));

            // Act
            var result1 = _gameWorld.MoveRequest(path);
            var result2 = _gameWorld.MoveRequest(path);

            // Assert
            Assert.AreEqual(2, result1.Count);
            Assert.IsTrue(result1[0].Command is WalkCommand);
            Assert.IsTrue(result1[1].Command is MoveCommand);

            Assert.AreEqual(2, result2.Count);
            Assert.IsTrue(result2[0].Command is WalkCommand);
            Assert.IsTrue(result2[1].Command is MoveCommand);
        }

        [TestMethod]
        public void Should_Walk_To_Position_And_Stop_At_Wall_With_NoMove()
        {
            // Arrange
            var wallPosition = new Point(2, 2);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 0));

            var path = _gameWorld.GetPathToPlayer(new Point(2, 2));

            // Act
            var result1 = _gameWorld.MoveRequest(path);
            var result2 = _gameWorld.MoveRequest(path);

            // Assert
            Assert.AreEqual(2, result1.Count);
            Assert.IsTrue(result1[0].Command is WalkCommand);
            Assert.IsTrue(result1[1].Command is MoveCommand);

            Assert.AreEqual(1, result2.Count);
            Assert.IsTrue(result2[0].Command is WalkCommand);
            Assert.IsTrue(((WalkCommand)result2[0].Command).CommandResult.Result == CommandResultEnum.NoMove);
        }

        [TestMethod]
        public void Should_Perform_NonMove_If_MoveDestination_Is_Wall_And_Only_One_Square()
        {
            // Arrange
            var wallPosition = new Point(1, 1);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 0));

            var path = _gameWorld.GetPathToPlayer(new Point(1, 1));

            // Act
            var result = _gameWorld.MoveRequest(path);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result[0].Command is WalkCommand);
        }

        [TestMethod]
        public void Walk_Player_Into_Adjacent_Monster_Via_MoveRequestDestination_Should_Attack()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 1)));

            var monster = _gameWorld.Monsters.Values.First();
            var healthBefore = monster.Health;

            var path = _gameWorld.GetPathToPlayer(new Point(0, 1));

            // Act
            _gameWorld.MoveRequest(path);

            // Assert
            Assert.AreEqual(0, _gameWorld.CommandCollection.GetLastCommands<MoveCommand>().Count);
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>().Count);
            Assert.AreEqual(new Point(0, 0), _gameWorld.Player.Position);

            var walkCommand = _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.Count);

            var attackCommand =
                _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.First() as MeleeAttackCommand;

            Assert.IsNotNull(attackCommand);
            Assert.AreEqual(CommandResultEnum.Success, attackCommand.CommandResult.Result);

            Assert.IsTrue(monster.Health < healthBefore);
            Assert.AreEqual("I hit the roach", attackCommand.CommandResult.Messages[0]);
        }

        [TestMethod]
        public void Walk_Player_Into_Monster_Via_MoveRequestDestination_Should_Stop_Adjacent_To_Monster()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));
            
            var spawnMonsterParams = new SpawnMonsterParams()
                .WithBreed("Roach")
                .AtPosition(new Point(0, 3))
                .WithState(MonsterState.Hunting);
            
            _gameWorld.SpawnMonster(spawnMonsterParams);

            var monster = _gameWorld.Monsters.Values.First();

            var path = _gameWorld.GetPathToPlayer(new Point(0, 3));

            // Act
            var result1 = _gameWorld.MoveRequest(path);
            var result2 = _gameWorld.MoveRequest(path);

            // Assert
            Assert.AreEqual(3, result1.Count);
            Assert.IsTrue(result1[0].Command is WalkCommand); // Player walk
            Assert.IsTrue(result1[1].Command is MoveCommand); // Player move
            Assert.IsTrue(result1[2].Command is MoveCommand); // Monster move

            Assert.AreEqual(1, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>().Count);
            Assert.AreEqual(new Point(0, 1), _gameWorld.Player.Position);
            Assert.AreEqual(new Point(0, 2), monster.Position);

            Assert.AreEqual(0, result2.Count);
        }

        [TestMethod]
        public void WalkCommand_Into_Ship_With_Repair_Parts_Should_Set_Victory_Flag()
        {
            // Arrange
            _gameWorld.NewGame();

            var shipRepairParts = _gameWorld.Items.Values.First(i => i.ItemType is ShipRepairParts);

            _gameWorld.Inventory.Add(shipRepairParts);

            // Act - player always starts directly underneath ship, moving up will enter the ship
            _gameWorld.MoveRequest(Direction.Up);

            // Assert
            Assert.IsTrue(_gameWorld.Player.IsVictorious);
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>().Count);

            var walkCommand = _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0];

            Assert.AreEqual(CommandResultEnum.NoMove, walkCommand.CommandResult.Result);
            Assert.AreEqual("I board my ship, make hasty repairs to critical parts and fire the engines! I have escaped!", walkCommand.CommandResult.Messages.First());
            Assert.AreEqual(0, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.Count);
        }
        
        [TestMethod]
        public void WalkCommand_Into_Ship_With_Repair_Parts_Using_Path_Should_Set_Victory_Flag()
        {
            // Arrange
            _gameWorld.NewGame();

            var shipRepairParts = _gameWorld.Items.Values.First(i => i.ItemType is ShipRepairParts);

            _gameWorld.Inventory.Add(shipRepairParts);

            // Act - player always starts directly underneath ship, moving up will enter the ship
            var path = _gameWorld.GetPathToPlayer(new Point(_gameWorld.Player.Position.X, _gameWorld.Player.Position.Y - 1));
            var result = _gameWorld.MoveRequest(path);

            // Assert
            Assert.IsTrue(_gameWorld.Player.IsVictorious);
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>().Count);

            var walkCommand = _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0];

            Assert.AreEqual(CommandResultEnum.NoMove, walkCommand.CommandResult.Result);
            Assert.AreEqual("I board my ship, make hasty repairs to critical parts and fire the engines! I have escaped!", walkCommand.CommandResult.Messages.First());
            Assert.AreEqual(0, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.Count);
        }

        [TestMethod]
        public void WalkCommand_Into_Ship_Without_Repair_Parts_Should_Not_Set_Victory_Flag()
        {
            // Arrange
            _gameWorld.NewGame();

            var shipRepairParts = _gameWorld.Items.Values.First(i => i.ItemType is ShipRepairParts);

            // Act - player always starts directly underneath ship, moving up will enter the ship
            _gameWorld.MoveRequest(Direction.Up);

            // Assert
            Assert.IsFalse(_gameWorld.Player.IsVictorious);
            Assert.AreEqual(1, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>().Count);

            var walkCommand = _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0];

            Assert.AreEqual(CommandResultEnum.NoMove, walkCommand.CommandResult.Result);
            Assert.AreEqual("I don't have the parts I need to repair my ship!", walkCommand.CommandResult.Messages.First());
            Assert.AreEqual(0, _gameWorld.CommandCollection.GetLastCommands<WalkCommand>()[0].CommandResult.SubsequentCommands.Count);
        }
    }
}
