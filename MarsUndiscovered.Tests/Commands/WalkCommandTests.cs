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
            NewGameWithCustomMapNoMonstersNoItems(_gameWorld);

            _gameWorld.Player.Position = new Point(0, 0);

            // Act
            _gameWorld.MoveRequest(Direction.Down);

            // Assert
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.Count());
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.MoveCommands.Count);
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.WalkCommands.Count);
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.AttackCommands.Count);
            Assert.AreEqual(new Point(0, 1), _gameWorld.Player.Position);

            var walkCommand = _gameWorld.HistoricalCommands.WalkCommands[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.Count);

            var moveCommand =
                _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.First() as MoveCommand;

            Assert.IsNotNull(moveCommand);
            Assert.AreEqual(CommandResultEnum.Success, moveCommand.CommandResult.Result);
        }

        [TestMethod]
        public void WalkCommand_Should_Return_NoMove_Result_For_Wall()
        {
            // Arrange
            var wallPosition = new Point(1, 0);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition });

            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator);

            _gameWorld.Player.Position = new Point(0, 0);

            // Act
            _gameWorld.MoveRequest(Direction.Right);

            // Assert
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.Count());
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.MoveCommands.Count);
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.WalkCommands.Count);
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.AttackCommands.Count);
            Assert.AreEqual(new Point(0, 0), _gameWorld.Player.Position);

            var walkCommand = _gameWorld.HistoricalCommands.WalkCommands[0];

            Assert.AreEqual(CommandResultEnum.NoMove, walkCommand.CommandResult.Result);
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.Count);
        }

        [TestMethod]
        public void WalkCommand_Player_Into_Monster_Should_Attack()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems(_gameWorld);

            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 1)));
            var monster = _gameWorld.Monsters.Values.First();
            var healthBefore = monster.Health;

            // Act
            _gameWorld.MoveRequest(Direction.Down);

            // Assert
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.Count());
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.MoveCommands.Count);
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.WalkCommands.Count);
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.AttackCommands.Count);
            Assert.AreEqual(new Point(0, 0), _gameWorld.Player.Position);

            var walkCommand = _gameWorld.HistoricalCommands.WalkCommands[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.Count);

            var attackCommand =
                _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.First() as MeleeAttackCommand;

            Assert.IsNotNull(attackCommand);
            Assert.AreEqual(CommandResultEnum.Success, attackCommand.CommandResult.Result);

            Assert.IsTrue(monster.Health < healthBefore);
            Assert.AreEqual("You hit the roach", attackCommand.CommandResult.Messages[0]);
        }

        [TestMethod]
        public void WalkCommand_Player_Into_Monster_On_Wall_Should_Attack()
        {
            // Arrange
            var wallPosition = new Point(0, 1);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition });

            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator);

            _gameWorld.Player.Position = new Point(0, 0);

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("TeslaTurret").AtPosition(wallPosition));

            var monster = _gameWorld.Monsters.Values.First();
            var healthBefore = monster.Health;

            // Act
            _gameWorld.MoveRequest(Direction.Down);

            // Assert
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.Count());
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.MoveCommands.Count);
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.WalkCommands.Count);
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.AttackCommands.Count);
            Assert.AreEqual(new Point(0, 0), _gameWorld.Player.Position);

            var walkCommand = _gameWorld.HistoricalCommands.WalkCommands[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.Count);

            var attackCommand =
                _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.First() as MeleeAttackCommand;

            Assert.IsNotNull(attackCommand);
            Assert.AreEqual(CommandResultEnum.Success, attackCommand.CommandResult.Result);

            Assert.IsTrue(monster.Health < healthBefore);
            Assert.AreEqual("You hit the tesla turret", attackCommand.CommandResult.Messages[0]);
        }

        [TestMethod]
        public void WalkCommand_With_NoDirection_With_Monster_Adjacent_Should_Result_In_Monster_Attacking_Player()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems(_gameWorld);

            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 1)));
            var monster = _gameWorld.Monsters.Values.First();
            var player = _gameWorld.Player;
            var healthBefore = player.Health;

            // Act
            var commandResults = _gameWorld.MoveRequest(Direction.None);

            // Assert
            Assert.AreEqual(new Point(0, 0), player.Position);
            Assert.AreEqual(new Point(0, 1), monster.Position);

            Assert.AreEqual(2, commandResults.Count);

            var walkCommandResult = commandResults[0];
            var attackCommandResult = commandResults[1];

            Assert.IsInstanceOfType(walkCommandResult.Command, typeof(WalkCommand));
            Assert.IsInstanceOfType(attackCommandResult.Command, typeof(MeleeAttackCommand));
            Assert.AreEqual("The roach hit you", attackCommandResult.Messages[0]);
            Assert.IsTrue(player.Health < healthBefore);
        }

        [TestMethod]
        public void Should_Save_Then_Load_Game_With_Commands()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems(_gameWorld);
            _gameWorld.Player.Position = new Point(0, 0);

            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            Assert.AreNotSame(_gameWorld, newGameWorld);
            Assert.AreEqual(2, newGameWorld.HistoricalCommands.Count());
            Assert.AreEqual(2, newGameWorld.HistoricalCommands.WalkCommands.Count);
            Assert.AreEqual(_gameWorld.Player.Position, newGameWorld.Player.Position);
            Assert.AreEqual(new Point(0, 2), newGameWorld.Player.Position);

            var walkCommand1 = newGameWorld.HistoricalCommands.WalkCommands[0];
            Assert.AreEqual(1, walkCommand1.TurnDetails.SequenceNumber);
            Assert.AreEqual(1, walkCommand1.TurnDetails.TurnNumber);

            var walkCommand2 = newGameWorld.HistoricalCommands.WalkCommands[1];
            Assert.AreEqual(3, walkCommand2.TurnDetails.SequenceNumber); // sequence number 2 is used in the move command, which is performed inside the walk command
            Assert.AreEqual(2, walkCommand2.TurnDetails.TurnNumber);
        }

        [TestMethod]
        public void Should_Walk_To_Position()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems(_gameWorld);

            _gameWorld.Player.Position = new Point(0, 0);

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

            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator);

            _gameWorld.Player.Position = new Point(0, 0);

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

            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator);

            _gameWorld.Player.Position = new Point(0, 0);

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
            NewGameWithCustomMapNoMonstersNoItems(_gameWorld);

            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 1)));

            var monster = _gameWorld.Monsters.Values.First();
            var healthBefore = monster.Health;

            var path = _gameWorld.GetPathToPlayer(new Point(0, 1));

            // Act
            _gameWorld.MoveRequest(path);

            // Assert
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.Count());
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.MoveCommands.Count);
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.WalkCommands.Count);
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.AttackCommands.Count);
            Assert.AreEqual(new Point(0, 0), _gameWorld.Player.Position);

            var walkCommand = _gameWorld.HistoricalCommands.WalkCommands[0];

            Assert.AreEqual(CommandResultEnum.Success, walkCommand.CommandResult.Result);
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.Count);

            var attackCommand =
                _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.First() as MeleeAttackCommand;

            Assert.IsNotNull(attackCommand);
            Assert.AreEqual(CommandResultEnum.Success, attackCommand.CommandResult.Result);

            Assert.IsTrue(monster.Health < healthBefore);
            Assert.AreEqual("You hit the roach", attackCommand.CommandResult.Messages[0]);
        }

        [TestMethod]
        public void Walk_Player_Into_Monster_Via_MoveRequestDestination_Should_Stop_Adjacent_To_Monster()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems(_gameWorld);

            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 3)));

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

            Assert.AreEqual(1, _gameWorld.HistoricalCommands.Count());
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.MoveCommands.Count);
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.WalkCommands.Count);
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.AttackCommands.Count);
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
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.Count());
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.WalkCommands.Count);

            var walkCommand = _gameWorld.HistoricalCommands.WalkCommands[0];

            Assert.AreEqual(CommandResultEnum.NoMove, walkCommand.CommandResult.Result);
            Assert.AreEqual("You board your ship, make hasty repairs to critical parts and fire the engines! You have escaped!", walkCommand.CommandResult.Messages.First());
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.Count);
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
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.Count());
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.WalkCommands.Count);

            var walkCommand = _gameWorld.HistoricalCommands.WalkCommands[0];

            Assert.AreEqual(CommandResultEnum.NoMove, walkCommand.CommandResult.Result);
            Assert.AreEqual("You board your ship, make hasty repairs to critical parts and fire the engines! You have escaped!", walkCommand.CommandResult.Messages.First());
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.Count);
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
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.Count());
            Assert.AreEqual(1, _gameWorld.HistoricalCommands.WalkCommands.Count);

            var walkCommand = _gameWorld.HistoricalCommands.WalkCommands[0];

            Assert.AreEqual(CommandResultEnum.NoMove, walkCommand.CommandResult.Result);
            Assert.AreEqual("You don't have the parts you need to repair your ship!", walkCommand.CommandResult.Messages.First());
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.Count);
        }
    }
}
