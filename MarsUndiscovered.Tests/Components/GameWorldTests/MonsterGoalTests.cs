using System.Text;
using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.Random;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Maps;
using NGenerics.Extensions;

using SadRogue.Primitives;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class MonsterGoalTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void Should_Move_Towards_Closest_Unexplored_Region_GoalMapWander()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);

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
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(2, 4)));

            var monster = _gameWorld.Monsters.Values.First();
            monster.UseGoalMapWander = true;

            _gameWorld.TestResetFieldOfView();
            monster.ResetFieldOfViewAndSeenTiles();

            // Act
            var result = monster.NextTurn(_gameWorld.CommandFactory).ToList();

            // Assert
            var moveCommand = result[0] as MoveCommand;
            Assert.IsNotNull(moveCommand);
            Assert.AreEqual(monster.Position, moveCommand.FromTo.Item1);
            Assert.AreEqual(new Point(1, 4), moveCommand.FromTo.Item2);
        }
        
        [TestMethod]
        public void Should_Move_Traverse_Out_Of_Caves_Using_GoalMapWander()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);

            _gameWorld.Player.Position = new Point(0, 0);
            
            var lines = new[]
            {
                "#########",
                "#........",
                "#########"
            };
            
            var mapTemplate = new MapTemplate(lines, 1, 1);

            foreach (var item in mapTemplate)
            {
                if (item.Char == '#')
                    _gameWorld.CreateWall(item.Point);
            }

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(2, 2)));

            var monster = _gameWorld.Monsters.Values.First();
            monster.UseGoalMapWander = true;
            monster.VisualRange = 2;

            _gameWorld.TestResetFieldOfView();
            monster.ResetFieldOfViewAndSeenTiles();

            // Act
            var result1 = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            var moveCommand1 = (MoveCommand)result1[0];
            monster.Position = moveCommand1.FromTo.Item2;
            
            var result2 = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            var moveCommand2 = (MoveCommand)result2[0];
            monster.Position = moveCommand2.FromTo.Item2;
            
            var result3 = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            var moveCommand3 = (MoveCommand)result3[0];
            monster.Position = moveCommand3.FromTo.Item2;
            
            var result4 = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            var moveCommand4 = (MoveCommand)result4[0];
            monster.Position = moveCommand4.FromTo.Item2;

            // Assert
            Assert.AreEqual(new Point(3, 2), moveCommand1.FromTo.Item2);
            Assert.AreEqual(new Point(4, 2), moveCommand2.FromTo.Item2);
            Assert.AreEqual(new Point(5, 2), moveCommand3.FromTo.Item2);
            Assert.AreEqual(new Point(6, 2), moveCommand4.FromTo.Item2);
        }
        
        [TestMethod]
        public void Should_Move_Traverse_Out_Of_Caves_After_Traversing_In_Using_GoalMapWander()
        {
            // Arrange
            var mapGenerator = new BlankMapGenerator(_gameWorld.GameObjectFactory);

            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator);

            _gameWorld.Player.Position = new Point(0, 0);
            
            var lines = new[]
            {
                "#########",
                "#........",
                "#########"
            };
            
            var mapTemplate = new MapTemplate(lines, 1, 1);

            foreach (var item in mapTemplate)
            {
                if (item.Char == '#')
                    _gameWorld.CreateWall(item.Point);
            }

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(4, 2)));

            var monster = _gameWorld.Monsters.Values.First();
            monster.UseGoalMapWander = true;
            monster.VisualRange = 2;

            _gameWorld.TestResetFieldOfView();
            monster.ResetFieldOfViewAndSeenTiles();

            // Act
            var result1 = monster.NextTurn(_gameWorld.CommandFactory).ToList();

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Map Turn 1");
            monster.GetGoalMap().AddToStringBuilder(stringBuilder);

            var moveCommand1 = (MoveCommand)result1[0];
            monster.Position = moveCommand1.FromTo.Item2;

            var result2 = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            
            stringBuilder.AppendLine("Map Turn 2");
            monster.GetGoalMap().AddToStringBuilder(stringBuilder);
            
            var moveCommand2 = (MoveCommand)result2[0];
            monster.Position = moveCommand2.FromTo.Item2;
            
            var result3 = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            
            stringBuilder.AppendLine("Map Turn 3");
            monster.GetGoalMap().AddToStringBuilder(stringBuilder);
            
            var moveCommand3 = (MoveCommand)result3[0];
            monster.Position = moveCommand3.FromTo.Item2;
            
            var result4 = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            
            stringBuilder.AppendLine("Map Turn 4");
            monster.GetGoalMap().AddToStringBuilder(stringBuilder);
            
            var moveCommand4 = (MoveCommand)result4[0];
            monster.Position = moveCommand4.FromTo.Item2;
            
            // Assert
            Console.Write(stringBuilder.ToString());
            
            Assert.AreEqual(new Point(3, 2), moveCommand1.FromTo.Item2);
            Assert.AreEqual(new Point(4, 2), moveCommand2.FromTo.Item2);
            Assert.AreEqual(new Point(5, 2), moveCommand3.FromTo.Item2);
            Assert.AreEqual(new Point(6, 2), moveCommand4.FromTo.Item2);
        }      
        
        [TestMethod]
        public void Monsters_Should_Update_Field_Of_View_Before_Calculating_Goals_Using_GoalMapWander()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);

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

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(2, 0)));
            
            var monster = _gameWorld.Monsters.Values.First();
            monster.UseGoalMapWander = true;

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
        public void Two_Monsters_Should_Not_Get_Stuck()
        {
            // Arrange
            var mapGenerator = new BlankMapGenerator(_gameWorld.GameObjectFactory);

            var levelGenerator = new TestLevelGenerator(_gameWorld, mapGenerator, 4, 7);
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator, levelGenerator);

            // ..#@
            // .###
            // ....
            // ###.
            // .M..
            // ###.
            // ..M.
            _gameWorld.Player.Position = new Point(3, 0);

            _gameWorld.CreateWall(2, 0);
            _gameWorld.CreateWall(1, 1);
            _gameWorld.CreateWall(2, 1);
            _gameWorld.CreateWall(3, 1);
            _gameWorld.CreateWall(0, 3);
            _gameWorld.CreateWall(1, 3);
            _gameWorld.CreateWall(2, 3);
            _gameWorld.CreateWall(0, 5);
            _gameWorld.CreateWall(1, 5);
            _gameWorld.CreateWall(2, 5);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(1, 4)));
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(2, 6)));

            var monster1 = _gameWorld.Monsters.Values.First();
            var monster2 = _gameWorld.Monsters.Values.Last();

            _gameWorld.TestResetFieldOfView();
            monster1.ResetFieldOfViewAndSeenTiles();
            monster2.ResetFieldOfViewAndSeenTiles();

            // Act
            var oldRandom = GlobalRandom.DefaultRNG;
            GlobalRandom.DefaultRNG = new KnownSeriesRandom(new int[] { 3, 6, 2, 4 });

            var result1 = monster1.NextTurn(_gameWorld.CommandFactory).ToList();
            var result2 = monster2.NextTurn(_gameWorld.CommandFactory).ToList();
            
            monster1.Position = ((MoveCommand)result1[0]).FromTo.Item2;
            monster2.Position = ((MoveCommand)result2[0]).FromTo.Item2;

            GlobalRandom.DefaultRNG = oldRandom;
            
            var result3 = monster1.NextTurn(_gameWorld.CommandFactory).ToList();
            var result4 = monster2.NextTurn(_gameWorld.CommandFactory).ToList();
            
            var result5 = monster1.NextTurn(_gameWorld.CommandFactory).ToList();
            var result6 = monster2.NextTurn(_gameWorld.CommandFactory).ToList();

            // Assert
            Assert.AreEqual(0 ,result3.Count);
            Assert.AreEqual(0 ,result4.Count);
            
            var moveCommand5 = result5[0] as MoveCommand;
            Assert.IsNotNull(moveCommand5);
            var moveCommand6 = result6[0] as MoveCommand;
            Assert.IsNotNull(moveCommand6);
        }

        [TestMethod]
        public void Should_Move_Towards_Unexplored_Region_Point_1_0()
        {
            // Arrange
            var mapGenerator = new BlankMapGenerator(_gameWorld.GameObjectFactory);

            var levelGenerator = new TestLevelGenerator(_gameWorld, mapGenerator, 4, 5);
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator, levelGenerator);

            // ..#@
            // P###
            // .P..
            // ..M.
            // ....
            // ....
            _gameWorld.Player.Position = new Point(3, 0);
            var wallPosition1 = new Point(1, 1);
            var wallPosition2 = new Point(2, 1);
            var wallPosition3 = new Point(3, 1);
            var wallPosition4 = new Point(0, 2);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.CreateWall(wallPosition2);
            _gameWorld.CreateWall(wallPosition3);
            _gameWorld.CreateWall(wallPosition3);
            _gameWorld.CreateWall(wallPosition4);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(2, 3)));

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.TestResetFieldOfView();
            monster.ResetFieldOfViewAndSeenTiles();

            // Act
            var result = monster.NextTurn(_gameWorld.CommandFactory).ToList();

            // Assert
            var moveCommand = result[0] as MoveCommand;
            Assert.IsNotNull(moveCommand);
            Assert.AreEqual(monster.Position, moveCommand.FromTo.Item1);
            Assert.AreEqual(new Point(1, 2), moveCommand.FromTo.Item2);
        }

        [TestMethod]
        public void First_Movement_Is_Up_Instead_Of_Diagonal_For_Deeper_Path()
        {
            // Arrange
            var mapGenerator = new BlankMapGenerator(_gameWorld.GameObjectFactory);

            var levelGenerator = new TestLevelGenerator(_gameWorld, mapGenerator, 4, 5);
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator, levelGenerator);

            // .PE@
            // P###
            // .P..
            // ..P.
            // ..M.
            // ....
            _gameWorld.Player.Position = new Point(3, 0);
            var wallPosition1 = new Point(1, 1);
            var wallPosition2 = new Point(2, 1);
            var wallPosition3 = new Point(3, 1);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.CreateWall(wallPosition2);
            _gameWorld.CreateWall(wallPosition3);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(2, 4)));

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.TestResetFieldOfView();
            monster.ResetFieldOfViewAndSeenTiles();

            // Act
            GlobalRandom.DefaultRNG = new KnownSeriesRandom(new int[] { 2, 0 });
            var result = monster.NextTurn(_gameWorld.CommandFactory).ToList();

            // Assert
            var moveCommand = result[0] as MoveCommand;
            Assert.IsNotNull(moveCommand);
                
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Map with Path");
            monster.CurrentMap.WalkabilityView.AddToStringBuilderWithPathGrid(stringBuilder, monster.GetWanderPath(), 1, (obj) => obj ? "." : "#");
            Console.Write(stringBuilder.ToString());

            var path = monster.GetWanderPath();
            var steps = path.StepsWithStart.ToList();
            var expectedSteps = new[]
            {
                new Point(2, 4),
                new Point(2, 3),
                new Point(1, 2),
                new Point(0, 1),
                new Point(1, 0),
                new Point(2, 0)
            };

            CollectionAssert.AreEquivalent(steps, expectedSteps);
        }
        
                [TestMethod]
        public void Should_Recalculate_Field_Of_View_When_All_Tiles_Seen_After_Reaching_End()
        {
            // Arrange
            var mapGenerator = new BlankMapGenerator(_gameWorld.GameObjectFactory);

            var levelGenerator = new TestLevelGenerator(_gameWorld, mapGenerator, 5, 5);
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator, levelGenerator);

            // ...#@    .P.#@
            // .#.##    P#M##
            // .#### => ?####
            // .M...    .....
            // .....    .....
            _gameWorld.Player.Position = new Point(4, 0);

            _gameWorld.CreateWall(3, 0);
            _gameWorld.CreateWall(1, 1);
            _gameWorld.CreateWall(3, 1);
            _gameWorld.CreateWall(4, 1);
            _gameWorld.CreateWall(1, 2);
            _gameWorld.CreateWall(2, 2);
            _gameWorld.CreateWall(3, 2);
            _gameWorld.CreateWall(4, 2);
            
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(1, 3)));

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.TestResetFieldOfView();
            monster.ResetFieldOfViewAndSeenTiles();
            
            var oldRandom = GlobalRandom.DefaultRNG;
            GlobalRandom.DefaultRNG = new KnownSeriesRandom(new int[] { 2, 1 });
            var result = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            monster.Position = ((MoveCommand)result[0]).FromTo.Item2;

            GlobalRandom.DefaultRNG = oldRandom;

            result = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            monster.Position = ((MoveCommand)result[0]).FromTo.Item2;
            result = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            monster.Position = ((MoveCommand)result[0]).FromTo.Item2;
            result = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            monster.Position = ((MoveCommand)result[0]).FromTo.Item2;

            // Act
            // The first NextTurn should result in no movement, the next turn should result in a recalculation and movement
            result = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            Assert.AreEqual(0, result.Count);
            
            result = monster.NextTurn(_gameWorld.CommandFactory).ToList();

            // Assert
            var moveCommand = result[0] as MoveCommand;
            Assert.IsNotNull(moveCommand);
                
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Map with Path");
            monster.CurrentMap.WalkabilityView.AddToStringBuilderWithPathGrid(stringBuilder, monster.GetWanderPath(), 1, (obj) => obj ? "." : "#");
            Console.Write(stringBuilder.ToString());

            var path = monster.GetWanderPath();
            var steps = path.StepsWithStart.ToList();
            var expectedSteps = new[]
            {
                new Point(2, 1),
                new Point(1, 0),
                new Point(0, 1)
            };
            
            Assert.IsTrue(steps.Count >= 3);

            // First 3 steps should have us moving out the cavern
            CollectionAssert.AreEquivalent(expectedSteps, steps.Take(3).ToList());
        }

        [TestMethod]
        public void Should_Progress_On_Path_Towards_Unexplored_Region()
        {
            // Arrange
            var mapGenerator = new BlankMapGenerator(_gameWorld.GameObjectFactory);

            var levelGenerator = new TestLevelGenerator(_gameWorld, mapGenerator, 4, 5);
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator, levelGenerator);

            // ..#@
            // .###
            // ....
            // ....
            // ..M.
            // ....
            _gameWorld.Player.Position = new Point(3, 0);

            _gameWorld.CreateWall(1, 1);
            _gameWorld.CreateWall(2, 1);
            _gameWorld.CreateWall(3, 1);
            _gameWorld.CreateWall(2, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(2, 4)));

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.TestResetFieldOfView();
            monster.ResetFieldOfViewAndSeenTiles();

            // Act
            var result1 = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            var result2 = monster.NextTurn(_gameWorld.CommandFactory).ToList();

            // Assert
            var moveCommand = result2[0] as MoveCommand;
            Assert.IsNotNull(moveCommand);
            Assert.AreEqual(monster.Position, moveCommand.FromTo.Item1);
            Assert.AreEqual(new Point(1, 3), moveCommand.FromTo.Item2);
        }
        
        [TestMethod]
        public void Should_Not_Move_If_Cannot_Find_Unexplored_Path()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);

            _gameWorld.Player.Position = new Point(0, 3);
            
            var lines = new[]
            {
                "#####",
                "#...#",
                "#####"
            };
            
            var mapTemplate = new MapTemplate(lines, 0, 0);

            foreach (var item in mapTemplate)
            {
                if (item.Char == '#')
                    _gameWorld.CreateWall(item.Point);
            }

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(2, 1)));

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.TestResetFieldOfView();
            monster.ResetFieldOfViewAndSeenTiles();

            // Act
            var result1 = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            var result2 = monster.NextTurn(_gameWorld.CommandFactory).ToList();

            // Assert
            Assert.AreEqual(0, result1.Count);
            Assert.AreEqual(0, result2.Count);
        }
       
        [TestMethod]
        public void Should_Move_Traverse_Out_Of_Caves()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);

            _gameWorld.Player.Position = new Point(0, 0);
            
            var lines = new[]
            {
                "#########",
                "#........",
                "#########"
            };
            
            var mapTemplate = new MapTemplate(lines, 1, 1);

            foreach (var item in mapTemplate)
            {
                if (item.Char == '#')
                    _gameWorld.CreateWall(item.Point);
            }

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(2, 2)));

            var monster = _gameWorld.Monsters.Values.First();
            monster.VisualRange = 2;

            _gameWorld.TestResetFieldOfView();
            monster.ResetFieldOfViewAndSeenTiles();

            // Act
            var result1 = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            var moveCommand1 = (MoveCommand)result1[0];
            monster.Position = moveCommand1.FromTo.Item2;
            
            var result2 = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            var moveCommand2 = (MoveCommand)result2[0];
            monster.Position = moveCommand2.FromTo.Item2;
            
            var result3 = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            var moveCommand3 = (MoveCommand)result3[0];
            monster.Position = moveCommand3.FromTo.Item2;
            
            var result4 = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            var moveCommand4 = (MoveCommand)result4[0];
            monster.Position = moveCommand4.FromTo.Item2;

            // Assert
            Assert.AreEqual(new Point(3, 2), moveCommand1.FromTo.Item2);
            Assert.AreEqual(new Point(4, 2), moveCommand2.FromTo.Item2);
            Assert.AreEqual(new Point(5, 2), moveCommand3.FromTo.Item2);
            Assert.AreEqual(new Point(6, 2), moveCommand4.FromTo.Item2);
        } 
        
        [TestMethod]
        public void Should_Move_Traverse_Out_Of_Caves_After_Traversing_In()
        {
            // Arrange
            var mapGenerator = new BlankMapGenerator(_gameWorld.GameObjectFactory);

            var levelGenerator = new TestLevelGenerator(_gameWorld, mapGenerator, 15, 5);
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator, levelGenerator);

            _gameWorld.Player.Position = new Point(0, 3);
            
            var lines = new[]
            {
                "#########",
                "#........",
                "#########"
            };
            
            var mapTemplate = new MapTemplate(lines, 0, 0);

            foreach (var item in mapTemplate)
            {
                if (item.Char == '#')
                    _gameWorld.CreateWall(item.Point);
            }

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(4, 1)));

            var monster = _gameWorld.Monsters.Values.First();
            monster.VisualRange = 2;

            _gameWorld.TestResetFieldOfView();
            monster.ResetFieldOfViewAndSeenTiles();

            // Act
            var result1 = monster.NextTurn(_gameWorld.CommandFactory).ToList();

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Map Turn 1");
            monster.CurrentMap.AddToStringBuilderWithPathGrid(stringBuilder, monster.GetWanderPath(), 1);

            var moveCommand1 = (MoveCommand)result1[0];
            monster.Position = moveCommand1.FromTo.Item2;

            var result2 = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            
            stringBuilder.AppendLine("Map Turn 2");
            monster.CurrentMap.AddToStringBuilderWithPathGrid(stringBuilder, monster.GetWanderPath(), 1);
            
            var moveCommand2 = (MoveCommand)result2[0];
            monster.Position = moveCommand2.FromTo.Item2;
            
            var result3 = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            
            stringBuilder.AppendLine("Map Turn 3");
            monster.CurrentMap.AddToStringBuilderWithPathGrid(stringBuilder, monster.GetWanderPath(), 1);
            
            var moveCommand3 = (MoveCommand)result3[0];
            monster.Position = moveCommand3.FromTo.Item2;
            
            var result4 = monster.NextTurn(_gameWorld.CommandFactory).ToList();
            
            stringBuilder.AppendLine("Map Turn 4");
            monster.CurrentMap.AddToStringBuilderWithPathGrid(stringBuilder, monster.GetWanderPath(), 1);
            
            var moveCommand4 = (MoveCommand)result4[0];
            monster.Position = moveCommand4.FromTo.Item2;
            
            // Assert
            Console.Write(stringBuilder.ToString());
            
            Assert.AreEqual(new Point(5, 1), moveCommand1.FromTo.Item2);
            Assert.AreEqual(new Point(6, 1), moveCommand2.FromTo.Item2);
            Assert.AreEqual(new Point(7, 1), moveCommand3.FromTo.Item2);
            Assert.AreEqual(new Point(8, 1), moveCommand4.FromTo.Item2);
        }

        [TestMethod]
        public void Should_Move_Towards_Player_When_Player_Is_Seen()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);

            _gameWorld.Player.Position = new Point(2, 0);
            var wallPosition1 = new Point(1, 5);
            var wallPosition2 = new Point(2, 5);
            var wallPosition3 = new Point(3, 5);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.CreateWall(wallPosition2);
            _gameWorld.CreateWall(wallPosition3);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(2, 4)));

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.TestResetFieldOfView();
            monster.ResetFieldOfViewAndSeenTiles();

            // Act
            var result = monster.NextTurn(_gameWorld.CommandFactory).ToList();

            // Assert
            var moveCommand = result[0] as MoveCommand;
            Assert.IsNotNull(moveCommand);
            Assert.AreEqual(monster.Position, moveCommand.FromTo.Item1);
            Assert.AreEqual(new Point(2, 3), moveCommand.FromTo.Item2);
        }

        [TestMethod]
        public void Should_Attack_Player_When_Adjacent_To_Player()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);

            _gameWorld.Player.Position = new Point(1, 1);

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(1, 0)));

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.TestResetFieldOfView();
            monster.ResetFieldOfViewAndSeenTiles();

            // Act
            var result = monster.NextTurn(_gameWorld.CommandFactory).ToList();

            // Assert
            var attackCommand = result[0] as MeleeAttackCommand;
            Assert.IsNotNull(attackCommand);
            Assert.AreSame(_gameWorld.Player, attackCommand.Target);
            Assert.AreSame(monster, attackCommand.Source);
        }

        [TestMethod]
        public void Monsters_Should_Stop_Acting_When_Player_Dies()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems(_gameWorld);

            _gameWorld.Player.Position = new Point(0, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(0, 1)));
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(1, 0)));
            _gameWorld.Player.Health = 1;

            // Act
            var result = _gameWorld.MoveRequest(Direction.Down);

            // Assert
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result[0].Command is WalkCommand); // Player walks into monster
            Assert.IsTrue(result[1].Command is MeleeAttackCommand); // Player attacks first roach
            Assert.IsTrue(result[2].Command is MeleeAttackCommand); // A roach attacks player
            Assert.IsTrue(result[3].Command is DeathCommand); // Player dies. Second roach does not act.

            var deathCommand = (DeathCommand)result.Last().Command;

            Assert.AreEqual("killed by a roach", deathCommand.KilledByMessage);
        }

        [TestMethod]
        public void Turrets_Should_Not_Move()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);

            _gameWorld.Player.Position = new Point(3, 3);
            var wallPosition1 = new Point(1, 1);
            var wallPosition2 = new Point(2, 2);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.CreateWall(wallPosition2);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("TeslaTurret").AtPosition(wallPosition1));

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.TestResetFieldOfView();
            monster.ResetFieldOfViewAndSeenTiles();

            // Act
            var result = monster.NextTurn(_gameWorld.CommandFactory).ToList();

            // Assert
            Assert.IsTrue(result.IsEmpty());
        }

        [TestMethod]
        public void Turrets_Should_Shoot_At_Player_When_Player_Is_Seen()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);

            _gameWorld.Player.Position = new Point(3, 3);
            var wallPosition1 = new Point(1, 1);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("TeslaTurret").AtPosition(wallPosition1));

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.TestResetFieldOfView();
            monster.ResetFieldOfViewAndSeenTiles();
            var playerHealth = _gameWorld.Player.Health;

            // Act
            var result = _gameWorld.TestNextTurn().ToList();

            // Assert
            var lightningAttackCommand = result[0].Command as LightningAttackCommand;
            Assert.IsNotNull(lightningAttackCommand);
            Assert.AreSame(_gameWorld.Player, lightningAttackCommand.Targets.Single());
            Assert.AreSame(monster, lightningAttackCommand.Source);
            Assert.AreEqual(playerHealth - monster.LightningAttack.Damage, _gameWorld.Player.Health);

            Assert.AreEqual(3, lightningAttackCommand.Path.Count);
            Assert.AreEqual(wallPosition1, lightningAttackCommand.Path[0]);
            Assert.AreEqual(new Point(2, 2), lightningAttackCommand.Path[1]);
            Assert.AreEqual(_gameWorld.Player.Position, lightningAttackCommand.Path[2]);
        }
    }
}
