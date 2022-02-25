using System.Linq;

using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Commands;
using MarsUndiscovered.Components;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class MonsterGoalTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void Should_Move_Towards_Closest_Unexplored_Region()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExits();

            _gameWorld.Player.Position = new Point(2, 0);
            var wallPosition1 = new Point(1, 1);
            var wallPosition2 = new Point(2, 1);
            var wallPosition3 = new Point(3, 1);
            var wallPosition4 = new Point(1, 5);
            var wallPosition5 = new Point(2, 5);
            var wallPosition6 = new Point(3, 5);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.CreateWall(wallPosition2);
            _gameWorld.CreateWall(wallPosition3);
            _gameWorld.CreateWall(wallPosition4);
            _gameWorld.CreateWall(wallPosition5);
            _gameWorld.CreateWall(wallPosition6);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(2, 4)));

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.ResetFieldOfView();
            monster.MonsterGoal.ResetFieldOfViewAndSeenTiles();

            // Act
            var result = monster.MonsterGoal.GetNextMove(_gameWorld);

            // Assert
            Assert.AreEqual(new Point(1, 4), monster.Position + result);
        }

        [TestMethod]
        public void Should_Move_Towards_Player_When_Player_Is_Seen()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExits();

            _gameWorld.Player.Position = new Point(2, 0);
            var wallPosition1 = new Point(1, 5);
            var wallPosition2 = new Point(2, 5);
            var wallPosition3 = new Point(3, 5);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.CreateWall(wallPosition2);
            _gameWorld.CreateWall(wallPosition3);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(2, 4)));

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.ResetFieldOfView();
            monster.MonsterGoal.ResetFieldOfViewAndSeenTiles();

            // Act
            var result = monster.MonsterGoal.GetNextMove(_gameWorld);

            // Assert
            Assert.AreEqual(new Point(2, 3), monster.Position + result);
        }

        [TestMethod]
        public void Should_Move_Towards_Player_When_Adjacent_To_Player()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExits();

            _gameWorld.Player.Position = new Point(1, 1);

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(1, 0)));

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.ResetFieldOfView();
            monster.MonsterGoal.ResetFieldOfViewAndSeenTiles();

            // Act
            var result = monster.MonsterGoal.GetNextMove(_gameWorld);

            // Assert
            Assert.AreEqual(new Point(1, 1), monster.Position + result);
        }

        [TestMethod]
        public void Monsters_Should_Stop_Acting_When_Player_Dies()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();

            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(0, 1)));
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(1, 0)));
            _gameWorld.Player.Health = 1;

            // Act
            var result = _gameWorld.MoveRequest(Direction.Down);

            // Assert
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result[0].Command is WalkCommand); // Player walks into monster
            Assert.IsTrue(result[1].Command is AttackCommand); // Player attacks first roach
            Assert.IsTrue(result[2].Command is AttackCommand); // A roach attacks player
            Assert.IsTrue(result[3].Command is DeathCommand); // Player dies. Second roach does not act.

            var deathCommand = (DeathCommand)result.Last().Command;

            Assert.AreEqual("killed by a roach", deathCommand.KilledByMessage);
        }

        [TestMethod]
        public void Monsters_Should_Update_Field_Of_View_Before_Calculating_Goals()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();

            _gameWorld.Player.Position = new Point(10, 10);

            var wallPosition1 = new Point(3, 0);
            var wallPosition2 = new Point(3, 1);
            var wallPosition3 = new Point(2, 1);
            var wallPosition4 = new Point(1, 1);
            var wallPosition5 = new Point(1, 2);
            var wallPosition6 = new Point(0, 2);
            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.CreateWall(wallPosition2);
            _gameWorld.CreateWall(wallPosition3);
            _gameWorld.CreateWall(wallPosition4);
            _gameWorld.CreateWall(wallPosition5);
            _gameWorld.CreateWall(wallPosition6);

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(2, 0)));

            // Act
            var result1 = _gameWorld.MoveRequest(Direction.Down);
            var result2 = _gameWorld.MoveRequest(Direction.Down);
            var result3 = _gameWorld.MoveRequest(Direction.Down);

            // Assert
            var moveCommand = result3[2].Command as MoveCommand;
            Assert.IsNotNull(moveCommand);

            Assert.AreEqual(moveCommand.GameObject, _gameWorld.Monsters.Values.First());
            Assert.AreEqual(moveCommand.FromTo.Item1, new Point(0, 1));

            // Current AI logic will cycle the monster between two squares. The monster is stuck in a wall and it has fully explored the area it is in.
            Assert.AreEqual(moveCommand.FromTo.Item2, new Point(0, 0));
        }

        [TestMethod]
        public void Turrets_Should_Not_Move()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExits();

            _gameWorld.Player.Position = new Point(3, 3);
            var wallPosition1 = new Point(1, 1);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.TeslaCoil).AtPosition(wallPosition1));

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.ResetFieldOfView();
            monster.MonsterGoal.ResetFieldOfViewAndSeenTiles();

            // Act
            var result = monster.MonsterGoal.GetNextMove(_gameWorld);

            // Assert
            Assert.AreEqual(Direction.None, result);
        }

        [TestMethod]
        public void Turrets_Should_Should_Shoot_At_Player_When_Player_Is_Seen()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExits();

            _gameWorld.Player.Position = new Point(3, 3);
            var wallPosition1 = new Point(1, 1);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.TeslaCoil).AtPosition(wallPosition1));

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.ResetFieldOfView();
            monster.MonsterGoal.ResetFieldOfViewAndSeenTiles();
            var playerHealth = _gameWorld.Player.Health;

            // Act
            var result = _gameWorld.NextTurn().ToList();

            // Assert
            Assert.AreEqual(CommandResultEnum.Success, result[0].Result);

            var lightningAttackCommand = result[0].Command as LightningAttackCommand;

            Assert.IsNotNull(lightningAttackCommand);
            Assert.AreEqual(playerHealth - monster.LightningAttack.Damage, _gameWorld.Player.Health);
        }
    }
}