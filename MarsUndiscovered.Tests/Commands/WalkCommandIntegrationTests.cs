using System.Linq;
using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Commands;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;
using MarsUndiscovered.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class WalkCommandIntegrationTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void WalkCommand_Should_Move_Player_For_Valid_Square()
        {
            // Arrange
            var blankMapGeneration = new BlankMapGenerator(
                Container.Resolve<IGameObjectFactory>(),
                Container.Resolve<IMapGenerator>()
            );

            _gameWorld.MapGenerator = blankMapGeneration;

            _gameWorld.NewGame();
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
            var gameObjectFactory = Container.Resolve<IGameObjectFactory>();

            var blankMapGeneration = new BlankMapGenerator(
                gameObjectFactory,
                Container.Resolve<IMapGenerator>()
            );

            _gameWorld.MapGenerator = blankMapGeneration;

            _gameWorld.NewGame();
            _gameWorld.Player.Position = new Point(0, 0);
            var wallPosition = new Point(1, 0);
            var wall = gameObjectFactory.CreateWall();

            wall.Position = wallPosition;

            _gameWorld.Map.SetTerrain(wall);

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
            var blankMapGeneration = new BlankMapGenerator(
                Container.Resolve<IGameObjectFactory>(),
                Container.Resolve<IMapGenerator>()
            );

            var blankMonsterGenerator = new BlankMonsterGenerator(
                Container.Resolve<IMonsterGenerator>()
            );

            _gameWorld.MapGenerator = blankMapGeneration;
            _gameWorld.MonsterGenerator = blankMonsterGenerator;

            _gameWorld.NewGame();

            _gameWorld.MonsterGenerator = blankMonsterGenerator.OriginalMonsterGenerator;

            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(0, 1)));
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
                _gameWorld.HistoricalCommands.WalkCommands[0].CommandResult.SubsequentCommands.First() as AttackCommand;

            Assert.IsNotNull(attackCommand);
            Assert.AreEqual(CommandResultEnum.Success, attackCommand.CommandResult.Result);

            Assert.IsTrue(monster.Health < healthBefore);
            Assert.AreEqual("You hit the roach", attackCommand.CommandResult.Messages[0]);
        }

        [TestMethod]
        public void WalkCommand_With_NoDirection_With_Monster_Adjacent_Should_Result_In_Monster_Attacking_Player()
        {
            // Arrange
            var blankMapGeneration = new BlankMapGenerator(
                Container.Resolve<IGameObjectFactory>(),
                Container.Resolve<IMapGenerator>()
            );

            var blankMonsterGenerator = new BlankMonsterGenerator(
                Container.Resolve<IMonsterGenerator>()
            );

            _gameWorld.MapGenerator = blankMapGeneration;
            _gameWorld.MonsterGenerator = blankMonsterGenerator;

            _gameWorld.NewGame();

            _gameWorld.MonsterGenerator = blankMonsterGenerator.OriginalMonsterGenerator;

            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(0, 1)));
            var monster = _gameWorld.Monsters.Values.First();
            var player = _gameWorld.Player;
            var healthBefore = player.Health;
            _gameWorld.RebuildGoalMaps();

            // Act
            var commandResults = _gameWorld.MoveRequest(Direction.None);

            // Assert
            Assert.AreEqual(new Point(0, 0), player.Position);
            Assert.AreEqual(new Point(0, 1), monster.Position);

            Assert.AreEqual(2, commandResults.Count);

            var walkCommandResult = commandResults[0];
            var attackCommandResult = commandResults[1];

            Assert.IsInstanceOfType(walkCommandResult.Command, typeof(WalkCommand));
            Assert.IsInstanceOfType(attackCommandResult.Command, typeof(AttackCommand));
            Assert.AreEqual("The roach hit you", attackCommandResult.Messages[0]);
            Assert.IsTrue(player.Health < healthBefore);
        }

        [TestMethod]
        public void Should_Save_Then_Load_Game_With_Commands()
        {
            // Arrange
            var blankMapGeneration = new BlankMapGenerator(
                Container.Resolve<IGameObjectFactory>(),
                Container.Resolve<IMapGenerator>()
            );

            _gameWorld.MapGenerator = blankMapGeneration;

            _gameWorld.NewGame();
            _gameWorld.Player.Position = new Point(0, 0);

            _gameWorld.MoveRequest(Direction.Down);
            _gameWorld.SaveGame("TestShouldSaveThenLoad", true);

            // Act
            var newGameWorld = (GameWorld)Container.Resolve<IGameWorld>();
            newGameWorld.LoadGame("TestShouldSaveThenLoad");

            // Assert
            Assert.AreNotSame(_gameWorld, newGameWorld);
            Assert.AreEqual(1, newGameWorld.HistoricalCommands.Count());
            Assert.AreEqual(0, newGameWorld.HistoricalCommands.MoveCommands.Count);
            Assert.AreEqual(1, newGameWorld.HistoricalCommands.WalkCommands.Count);
            Assert.AreEqual(0, newGameWorld.HistoricalCommands.AttackCommands.Count);
            Assert.AreEqual(_gameWorld.Player.Position, newGameWorld.Player.Position);
            Assert.AreEqual(new Point(0, 1), newGameWorld.Player.Position);
        }
    }
}