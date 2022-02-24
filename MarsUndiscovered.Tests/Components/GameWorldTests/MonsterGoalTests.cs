using System.Linq;

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
    }
}