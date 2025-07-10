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
        public void Monster_Wander_Into_Waiting_Player_Should_Immediately_Change_To_Hunting_Status()
        {
            // Arrange
            var wallPosition1 = new Point(1, 0);
            var wallPosition2 = new Point(1, 1);
            
            var mapGenerator = new SpecificMapGenerator(
                _gameWorld.GameObjectFactory,
                new[] { wallPosition1, wallPosition2 });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 1));

            Breed.Breeds["Roach"].DetectionRange = 0;
            
            var spawnMonsterParams = new SpawnMonsterParams()
                .WithBreed("Roach")
                .AtPosition(new Point(0, 0))
                .WithState(MonsterState.Wandering);
            
            _gameWorld.SpawnMonster(spawnMonsterParams);
            
            var monster = _gameWorld.Monsters.Values.First();
            var player = _gameWorld.Player;

            // Act
            var commandResults = _gameWorld.MoveRequest(Direction.None);

            // Assert
            Assert.AreEqual(monster.MonsterState, MonsterState.Hunting);
            
            Assert.AreEqual(new Point(0, 1), player.Position);
            Assert.AreEqual(new Point(0, 0), monster.Position);

            // Monster will not attack this turn, state just switches to hunting
            Assert.AreEqual(1, commandResults.Count);

            var waitCommandResult = commandResults[0];
            Assert.IsInstanceOfType(waitCommandResult.Command, typeof(WaitCommand));
        }        

        [TestMethod]
        public void Should_Save_Then_Load_Game_With_Wait_Commands()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld, playerPosition: new Point(0, 0));

            _gameWorld.MoveRequest(Direction.None);
            _gameWorld.MoveRequest(Direction.None);
            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            Assert.AreNotSame(_gameWorld, newGameWorld);

            var waitCommands = _gameWorld.CommandCollection.GetCommands<WaitCommand>();

            Assert.AreEqual(2, waitCommands.Count);
            Assert.AreEqual(_gameWorld.Player.Position, newGameWorld.Player.Position);
            Assert.AreEqual(new Point(0, 0), newGameWorld.Player.Position);

            var waitCommand1 = waitCommands[0];
            Assert.AreEqual(1, waitCommand1.TurnDetails.SequenceNumber);
            Assert.AreEqual(1, waitCommand1.TurnDetails.TurnNumber);

            var waitCommand2 = waitCommands[1];
            Assert.AreEqual(2, waitCommand2.TurnDetails.SequenceNumber);
            Assert.AreEqual(2, waitCommand2.TurnDetails.TurnNumber);
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
