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
        public void WanderState_Should_Move_Towards_Undiscovered_Region()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExits();

            _gameWorld.Player.Position = new Point(0, 0);
            var wallPosition1 = new Point(0, 1);
            var wallPosition2 = new Point(1, 1);
            var wallPosition3 = new Point(2, 1);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.CreateWall(wallPosition2);
            _gameWorld.CreateWall(wallPosition3);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(1, 0)));

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.ResetFieldOfView();
            monster.MonsterGoal.ResetFieldOfViewAndSeenTiles();

            // Act
            var result = monster.MonsterGoal.GetNextMove(_gameWorld);

            // Assert
            Assert.AreEqual(new Point(2, 0), monster.Position + result);
        }

        [TestMethod]
        public void HuntState_Should_Move_Towards_Player()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExits();

            _gameWorld.Player.Position = new Point(0, 0);
            var wallPosition1 = new Point(0, 1);
            var wallPosition2 = new Point(1, 1);
            var wallPosition3 = new Point(2, 1);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.CreateWall(wallPosition2);
            _gameWorld.CreateWall(wallPosition3);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(new Point(1, 0)));

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.ResetFieldOfView();
            monster.MonsterGoal.ResetFieldOfViewAndSeenTiles();
            monster.MonsterGoal.ChangeStateToHunt();

            // Act
            var result = monster.MonsterGoal.GetNextMove(_gameWorld);

            // Assert
            Assert.AreEqual(new Point(0, 0), monster.Position + result);
        }
    }
}