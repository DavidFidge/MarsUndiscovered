using MarsUndiscovered.Game.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class AutoExploreTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void AutoExplore_Should_Move_Towards_Unknown_Space()
        {
            // Arrange
            var wallPosition1 = new Point(1, 0);
            var wallPosition2 = new Point(1, 1);
            var wallPosition3 = new Point(1, 2);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition1, wallPosition2, wallPosition3 });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 0));

            _gameWorld.TestResetFieldOfView();

            // Act
            var result = _gameWorld.AutoExploreRequest();

            // Assert
            Assert.AreEqual(new Point(0, 1), _gameWorld.Player.Position);

            Assert.AreEqual(4, result.Path.LengthWithStart);
            Assert.AreEqual(new Point(0, 1), result.Path.GetStepWithStart(0));
            Assert.AreEqual(new Point(0, 2), result.Path.GetStepWithStart(1));
            Assert.AreEqual(new Point(1, 3), result.Path.GetStepWithStart(2));
            Assert.AreEqual(new Point(2, 3), result.Path.GetStepWithStart(3));
            Assert.IsFalse(result.MovementInterrupted);
        }

        [TestMethod]
        public void AutoExplore_Should_Prioritise_Picking_Up_Undropped_Items()
        {
            // Arrange
            var wallPosition1 = new Point(1, 0);
            var wallPosition2 = new Point(1, 1);
            var wallPosition3 = new Point(1, 2);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition1, wallPosition2, wallPosition3 });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 0));

            var itemPosition = new Point(0, 5);

            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(itemPosition));

            _gameWorld.TestResetFieldOfView();

            // Act
            var result1 = _gameWorld.AutoExploreRequest();
            var result2 = _gameWorld.AutoExploreRequest();
            var result3 = _gameWorld.AutoExploreRequest();
            var result4 = _gameWorld.AutoExploreRequest();
            var result5 = _gameWorld.AutoExploreRequest();

            // Assert
            Assert.AreEqual(new Point(0, 5), _gameWorld.Player.Position);
        }

        [TestMethod]
        public void AutoExplore_Should_Ignore_Items_That_Have_Been_Picked_Up_Prior()
        {
            // Arrange
            var wallPosition1 = new Point(1, 0);
            var wallPosition2 = new Point(1, 1);
            var wallPosition3 = new Point(1, 2);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition1, wallPosition2, wallPosition3 });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 0));
            _gameWorld.Player.VisualRange = 1000;

            var itemPosition = new Point(0, 5);

            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(itemPosition));
            _gameWorld.Items.Values.First().HasBeenDropped = true;

            _gameWorld.TestResetFieldOfView();

            // Act
            var result1 = _gameWorld.AutoExploreRequest();
            var result2 = _gameWorld.AutoExploreRequest();
            var result3 = _gameWorld.AutoExploreRequest();
            var result4 = _gameWorld.AutoExploreRequest();
            var result5 = _gameWorld.AutoExploreRequest();
            var result6 = _gameWorld.AutoExploreRequest();

            // Assert
            Assert.AreEqual(new Point(2, 2), _gameWorld.Player.Position);
        }

        [TestMethod]
        public void AutoExplore_Should_Go_To_Exit_When_Map_Explored()
        {
            // Arrange
            var wallPosition1 = new Point(1, 0);
            var wallPosition2 = new Point(1, 1);
            var wallPosition3 = new Point(1, 2);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition1, wallPosition2, wallPosition3 });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 0));
            _gameWorld.Player.VisualRange = 1000;

            var mapExitPosition = new Point(0, 5);

            _gameWorld.SpawnMapExit(new SpawnMapExitParams().AtPosition(mapExitPosition).WithMapExitType(MapExitType.MapExitDown));
            _gameWorld.TestResetFieldOfView();

            // Act
            var result1 = _gameWorld.AutoExploreRequest();
            var result2 = _gameWorld.AutoExploreRequest();
            var result3 = _gameWorld.AutoExploreRequest();
            var result4 = _gameWorld.AutoExploreRequest();

            // Assert
            Assert.AreEqual(new Point(1, 3), result3.Path.GetStepWithStart(0));
            Assert.AreEqual(new Point(2, 2), result3.Path.GetStepWithStart(1));
            Assert.AreEqual(new Point(2, 1), result3.Path.GetStepWithStart(2));
            Assert.AreEqual(new Point(2, 0), result3.Path.GetStepWithStart(3));

            // Once map is revealed then the next position is the map exit
            Assert.AreEqual(new Point(2, 2), result4.Path.GetStepWithStart(0));
            Assert.AreEqual(new Point(2, 3), result4.Path.GetStepWithStart(1));
            Assert.AreEqual(new Point(1, 4), result4.Path.GetStepWithStart(2));
            Assert.AreEqual(mapExitPosition, result4.Path.GetStepWithStart(3));
            Assert.IsFalse(result4.MovementInterrupted);
        }

        [TestMethod]
        public void AutoExplore_Should_Not_Go_To_Exit_When_Fallback_To_Map_Exit_Is_False()
        {
            // Arrange
            var wallPosition1 = new Point(1, 0);
            var wallPosition2 = new Point(1, 1);
            var wallPosition3 = new Point(1, 2);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition1, wallPosition2, wallPosition3 });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 0));
            _gameWorld.Player.VisualRange = 1000;

            var mapExitPosition = new Point(0, 5);

            _gameWorld.SpawnMapExit(new SpawnMapExitParams().AtPosition(mapExitPosition).WithMapExitType(MapExitType.MapExitDown));
            _gameWorld.TestResetFieldOfView();

            // Act
            var result1 = _gameWorld.AutoExploreRequest(false);
            var result2 = _gameWorld.AutoExploreRequest(false);
            var result3 = _gameWorld.AutoExploreRequest(false);
            var result4 = _gameWorld.AutoExploreRequest(false);
            var result5 = _gameWorld.AutoExploreRequest(false);
            var result6 = _gameWorld.AutoExploreRequest(false);

            // Assert
            Assert.AreEqual(new Point(2, 2), _gameWorld.Player.Position);
        }

        [TestMethod]
        public void AutoExplore_Should_Not_Move_If_Stuck_In_Walls()
        {
            // Arrange
            var wallPositions = new List<Point>();

            for (var y = 0; y < 2; y++)
            {
                for (var x = 0; x < 2; x++)
                {
                    var point = new Point(x, y);

                    if (point != new Point(0, 0))
                        wallPositions.Add(new Point(x, y));
                }
            }

            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, wallPositions);

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 0));

            _gameWorld.TestResetFieldOfView();

            // Act
            var result = _gameWorld.AutoExploreRequest();

            // Assert
            Assert.AreEqual(new Point(0, 0), _gameWorld.Player.Position);
            Assert.IsFalse(result.MovementInterrupted);
        }

        [TestMethod]
        public void AutoExplore_Should_Not_Get_Stuck_On_Wall()
        {
            // Arrange
            var wallPositions = new List<Point>();

            for (var y = 1; y < 6; y++)
            {
                for (var x = 1; x < 6; x++)
                {
                    wallPositions.Add(new Point(x, y));
                }
            }

            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, wallPositions);

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 0));
            _gameWorld.Player.VisualRange = 1000;

            _gameWorld.TestResetFieldOfView();

            // Act
            var result1 = _gameWorld.AutoExploreRequest();
            var result2 = _gameWorld.AutoExploreRequest();
            var result3 = _gameWorld.AutoExploreRequest();
            var result4 = _gameWorld.AutoExploreRequest();
            var result5 = _gameWorld.AutoExploreRequest();
            var result6 = _gameWorld.AutoExploreRequest();
            var result7 = _gameWorld.AutoExploreRequest();
            var result8 = _gameWorld.AutoExploreRequest();
            var result9 = _gameWorld.AutoExploreRequest();
            var result10 = _gameWorld.AutoExploreRequest();
            var result11 = _gameWorld.AutoExploreRequest();

            // Assert
            // Map is entirely explored once the player reaches 6, 5
            Assert.AreEqual(new Point(6, 5), _gameWorld.Player.Position);

            // It takes 11 move steps to reach here, check the last 2
            Assert.AreEqual(3, result10.Path.LengthWithStart);
            Assert.AreEqual(new Point(5, 6), result10.Path.Start);

            Assert.AreEqual(1, result11.Path.LengthWithStart);
            Assert.AreEqual(new Point(6, 5), result11.Path.Start);
        }

        [TestMethod]
        public void AutoExplore_Should_Stay_Still_If_No_Place_To_Explore()
        {
            // Arrange
            var wallPosition1 = new Point(1, 0);
            var wallPosition2 = new Point(1, 1);
            var wallPosition3 = new Point(1, 2);
            var wallPosition4 = new Point(0, 2);

            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition1, wallPosition2, wallPosition3, wallPosition4 });
            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 0));

            var playerPosition = _gameWorld.Player.Position;

            _gameWorld.TestResetFieldOfView();

            // Act
            var result = _gameWorld.AutoExploreRequest();

            // Assert
            Assert.AreEqual(playerPosition, _gameWorld.Player.Position);
            Assert.AreEqual(0, result.Path.LengthWithStart);
            Assert.IsFalse(result.MovementInterrupted);
        }

        [TestMethod]
        public void AutoExplore_Should_Attack_Adjacent_Monsters_Before_Exploring()
        {
            // Arrange
            var wallPositions = new List<Point>();

            for (var y = 1; y < 6; y++)
            {
                for (var x = 1; x < 6; x++)
                {
                    wallPositions.Add(new Point(x, y));
                }
            }

            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, wallPositions);

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 0));

            var playerPosition = _gameWorld.Player.Position;

            var monsterPosition = new Point(1, 0);
            
            var spawnMonsterParams = new SpawnMonsterParams()
                .WithBreed("Roach")
                .AtPosition(monsterPosition)
                .WithState(MonsterState.Hunting);
            
            _gameWorld.SpawnMonster(spawnMonsterParams);
            _gameWorld.TestResetFieldOfView();

            // Act
            var result = _gameWorld.AutoExploreRequest();

            // Assert
            Assert.AreEqual(2, result.Path.LengthWithStart);
            Assert.AreEqual(playerPosition, result.Path.Start);
            Assert.AreEqual(monsterPosition, result.Path.End);
            Assert.IsTrue(result.MovementInterrupted);
        }

        [TestMethod]
        public void AutoExplore_Should_Stop_Movement_If_Attacked_By_Lightning()
        {
            // Arrange
            var wallPosition = new Point(0, 0);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(3, 3));
            _gameWorld.Player.VisualRange = 1000;

            var mapExitPosition = new Point(5, 5);

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("TeslaTurret").AtPosition(wallPosition));
            _gameWorld.SpawnMapExit(new SpawnMapExitParams().AtPosition(mapExitPosition).WithMapExitType(MapExitType.MapExitDown));
            _gameWorld.TestResetFieldOfView();

            // Act
            var result = _gameWorld.AutoExploreRequest();

            // Assert
            Assert.AreEqual(2, result.Path.LengthWithStart);
            Assert.AreEqual(new Point(4, 4), result.Path.Start);
            Assert.AreEqual(mapExitPosition, result.Path.End);
            Assert.IsTrue(result.MovementInterrupted);
        }

        [TestMethod]
        public void AutoExplore_Should_Stop_Movement_When_New_Monsters_Come_Into_View()
        {
            // Arrange
            var wallPosition = new Point(0, 1);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 2));

            var monsterPosition = new Point(0, 0);
            var mapExitPosition = new Point(5, 10);

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(monsterPosition));
            _gameWorld.SpawnMapExit(new SpawnMapExitParams().AtPosition(mapExitPosition).WithMapExitType(MapExitType.MapExitDown));
            _gameWorld.TestResetFieldOfView();

            // Act
            var result = _gameWorld.AutoExploreRequest();

            // Assert
            Assert.IsTrue(result.MovementInterrupted);
        }

        [TestMethod]
        public void AutoExplore_Should_Not_Stop_Movement_When_Existing_Monsters_Are_In_View()
        {
            // Arrange
            var wallPosition = new Point(0, 1);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 10));

            var monsterPosition = new Point(0, 0);
            var mapExitPosition = new Point(5, 10);

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(monsterPosition));
            _gameWorld.SpawnMapExit(new SpawnMapExitParams().AtPosition(mapExitPosition).WithMapExitType(MapExitType.MapExitDown));
            _gameWorld.TestResetFieldOfView();

            // Act
            var result1 = _gameWorld.AutoExploreRequest();
            var result2 = _gameWorld.AutoExploreRequest();

            // Assert
            Assert.IsFalse(result2.MovementInterrupted);
        }

        [TestMethod]
        public void AutoExplore_Should_Not_Stop_Movement_When_Existing_Monster_Goes_Out_Of_View()
        {
            // Arrange
            var wallPosition = new Point(0, 1);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition });

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, playerPosition: new Point(0, 10));

            var monsterPosition = new Point(0, 0);
            var mapExitPosition = new Point(5, 10);

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(monsterPosition));
            _gameWorld.SpawnMapExit(new SpawnMapExitParams().AtPosition(mapExitPosition).WithMapExitType(MapExitType.MapExitDown));
            _gameWorld.TestResetFieldOfView();

            // Act
            var result1 = _gameWorld.AutoExploreRequest();
            var result2 = _gameWorld.AutoExploreRequest();
            
            _gameWorld.Monsters.Values.First().IsDead = true;

            var result3 = _gameWorld.AutoExploreRequest();

            // Assert
            Assert.IsFalse(result3.MovementInterrupted);
        }
    }
}