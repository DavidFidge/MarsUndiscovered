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
            var wallPosition1 = new Point(1, 1);
            var wallPosition2 = new Point(2, 1);
            var wallPosition3 = new Point(3, 1);
            var wallPosition4 = new Point(1, 5);
            var wallPosition5 = new Point(2, 5);
            var wallPosition6 = new Point(3, 5);

            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition1, wallPosition2, wallPosition3, wallPosition4, wallPosition5, wallPosition6 });

            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator);

            _gameWorld.Player.Position = new Point(2, 0);

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
            var lines = new[]
            {
                "#########",
                "#........",
                "#########"
            };

            var mapTemplate = new MapTemplate(lines, 1, 1);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator);

            _gameWorld.Player.Position = new Point(0, 0);

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
            var lines = new[]
            {
                "#########",
                "#........",
                "#########"
            };
            
            var mapTemplate = new MapTemplate(lines, 1, 1);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator);

            _gameWorld.Player.Position = new Point(0, 0);

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
            var wallPosition1 = new Point(3, 0);
            var wallPosition2 = new Point(3, 1);
            var wallPosition3 = new Point(2, 1);
            var wallPosition4 = new Point(1, 1);
            var wallPosition5 = new Point(1, 2);
            var wallPosition6 = new Point(0, 2);

            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition1, wallPosition2, wallPosition3, wallPosition4, wallPosition5, wallPosition6 });
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator);

            _gameWorld.Player.Position = new Point(10, 10);

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
            var walls = new Point[]
            {
                new (2, 0),
                new (1, 1),
                new (2, 1),
                new (3, 1),
                new (0, 3),
                new (1, 3),
                new (2, 3),
                new (0, 5),
                new (1, 5),
                new (2, 5)
            };
            
            var mapGenerator = new SpecificMapGenerator(
                _gameWorld.GameObjectFactory,
                walls
            );
            
            var levelGenerator = new TestLevelGenerator(_gameWorld, mapGenerator, 4, 7);
            levelGenerator.PlayerPosition = new Point(3, 0);

            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator, levelGenerator);

            // ..#@
            // .###
            // ....
            // ###.
            // .M..
            // ###.
            // ..M.

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
            var wallPosition1 = new Point(1, 1);
            var wallPosition2 = new Point(2, 1);
            var wallPosition3 = new Point(3, 1);
            var wallPosition4 = new Point(0, 2);

            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition1, wallPosition2, wallPosition3, wallPosition4 });
            var levelGenerator = new TestLevelGenerator(_gameWorld, mapGenerator, 4, 5);
            levelGenerator.PlayerPosition = new Point(3, 0);

            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator, levelGenerator);

            // ..#@
            // P###
            // .P..
            // ..M.
            // ....
            // ....
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
            var wallPosition1 = new Point(1, 1);
            var wallPosition2 = new Point(2, 1);
            var wallPosition3 = new Point(3, 1);

            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition1, wallPosition2, wallPosition3 });

            var levelGenerator = new TestLevelGenerator(_gameWorld, mapGenerator, 4, 5);
            levelGenerator.PlayerPosition = new Point(3, 0);

            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator, levelGenerator);

            // .PE@
            // P###
            // .P..
            // ..P.
            // ..M.
            // ....
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
            var walls = new Point[]
            {
                new (3, 0),
                new (1, 1),
                new (3, 1),
                new (4, 1),
                new (1, 2),
                new (2, 2),
                new (3, 2),
                new (4, 2)
            };
            
            var mapGenerator = new SpecificMapGenerator(
                _gameWorld.GameObjectFactory,
                walls
            );

            var levelGenerator = new TestLevelGenerator(_gameWorld, mapGenerator, 5, 5);
            levelGenerator.PlayerPosition = new Point(4, 0);

            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator, levelGenerator);

            // ...#@    .P.#@
            // .#.##    P#M##
            // .#### => ?####
            // .M...    .....
            // .....    .....
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
            var walls = new Point[]
            {
                new (1, 1),
                new (2, 1),
                new (3, 1),
                new (2, 0)
            };
            
            var mapGenerator = new SpecificMapGenerator(
                _gameWorld.GameObjectFactory,
                walls
            );

            var levelGenerator = new TestLevelGenerator(_gameWorld, mapGenerator, 4, 5);
            levelGenerator.PlayerPosition = new Point(3, 0);

            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator, levelGenerator);

            // ..#@
            // .###
            // ....
            // ....
            // ..M.
            // ....
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
            var lines = new[]
            {
                "#####",
                "#...#",
                "#####"
            };
            
            var mapTemplate = new MapTemplate(lines, 0, 0);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator);

            _gameWorld.Player.Position = new Point(0, 3);

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
        public void Should_Move_Towards_Leader()
        {
            // Arrange
            var lines = new[]
            {
                ".#.......",
                "##.......",
                ".........",
                ".........",
                "...#.#...",
                "...#.#...",
                "...#.#...",
                "...#.#...",
                "...#.#...",
                ".........",
                ".........",
                ".........",
                "........."
            };

            var mapTemplate = new MapTemplate(lines, 0, 0);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator);

            _gameWorld.Player.Position = new Point(0, 0);

            var monsterLeader = new SpawnMonsterParams()
                .WithBreed("RepairDroid")
                .AtPosition(new Point(4, 6));
            
            _gameWorld.SpawnMonster(monsterLeader);
            
            var monsterFollower1 = new SpawnMonsterParams()
                .WithBreed("RepairDroid")
                .WithLeader(monsterLeader.Result.ID)
                .AtPosition(new Point(4, 0));
            
            _gameWorld.SpawnMonster(monsterFollower1);
            
            var monsterFollower2 = new SpawnMonsterParams()
                .WithBreed("RepairDroid")
                .WithLeader(monsterLeader.Result.ID)
                .AtPosition(new Point(4, 12));
            
            _gameWorld.SpawnMonster(monsterFollower2);

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.TestResetFieldOfView();
            monster.ResetFieldOfViewAndSeenTiles();

            // Act
            var result1 = monsterFollower1.Result.NextTurn(_gameWorld.CommandFactory).ToList();
            var result2 = monsterFollower2.Result.NextTurn(_gameWorld.CommandFactory).ToList();

            // Assert
            var moveCommand1 = (MoveCommand)result1[0];
            var moveCommand2 = (MoveCommand)result2[0];
            
            Assert.AreEqual(new Point(4, 1), moveCommand1.FromTo.Item2);
            Assert.AreEqual(new Point(4, 11), moveCommand2.FromTo.Item2);
        }
       
        [TestMethod]
        public void Should_Move_Traverse_Out_Of_Caves()
        {
            // Arrange
            var lines = new[]
            {
                "#########",
                "#........",
                "#########"
            };
            
            var mapTemplate = new MapTemplate(lines, 1, 1);

            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator);

            _gameWorld.Player.Position = new Point(0, 0);

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
            var lines = new[]
            {
                "#########",
                "#........",
                "#########"
            };
            
            var mapTemplate = new MapTemplate(lines, 0, 0);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            var levelGenerator = new TestLevelGenerator(_gameWorld, mapGenerator, 15, 5);
            levelGenerator.PlayerPosition = new Point(0, 3);
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator, levelGenerator);

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(4, 1)));

            var monster = _gameWorld.Monsters.Values.First();
            monster.VisualRange = 4;

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
            var wallPosition1 = new Point(1, 5);
            var wallPosition2 = new Point(2, 5);
            var wallPosition3 = new Point(3, 5);

            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition1, wallPosition2, wallPosition3 });
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator);

            _gameWorld.Player.Position = new Point(2, 0);

            var spawnMonsterParams = new SpawnMonsterParams()
                .WithBreed("Roach")
                .AtPosition(new Point(2, 4))
                .WithState(MonsterState.Hunting);
            
            _gameWorld.SpawnMonster(spawnMonsterParams);

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

            var spawnMonsterParams = new SpawnMonsterParams()
                .WithBreed("Roach")
                .AtPosition(new Point(1, 0))
                .WithState(MonsterState.Hunting);
            
            _gameWorld.SpawnMonster(spawnMonsterParams);

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
        [DataRow(true)]
        [DataRow(false)]
        public void Should_Attack_Player_When_Has_Line_Attack(bool isAdjacent)
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);

            _gameWorld.Player.Position = new Point(1, 2);

            var y = isAdjacent ? 1 : 0;
            var spawnMonsterParams = new SpawnMonsterParams()
                .WithBreed("CleaningDroid")
                .AtPosition(new Point(1, y))
                .WithState(MonsterState.Hunting);
            
            _gameWorld.SpawnMonster(spawnMonsterParams);

            var monster = _gameWorld.Monsters.Values.First();

            _gameWorld.TestResetFieldOfView();
            monster.ResetFieldOfViewAndSeenTiles();

            // Act
            var result = monster.NextTurn(_gameWorld.CommandFactory).ToList();

            // Assert
            var attackCommand = result[0] as LineAttackCommand;
            Assert.IsNotNull(attackCommand);
            Assert.AreEqual(1, attackCommand.Targets.Count);
            Assert.AreSame(_gameWorld.Player, attackCommand.Targets[0]);
            Assert.AreSame(monster, attackCommand.Source);
        }
        
        [TestMethod]
        public void Can_Kill_Other_Monsters_With_LineAttack()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);

            _gameWorld.Player.Position = new Point(0, 0);

            var spawnMonsterParams1 = new SpawnMonsterParams()
                .WithBreed("CleaningDroid")
                .AtPosition(new Point(0, 1))
                .WithState(MonsterState.Hunting);
            
            var spawnMonsterParams2 = new SpawnMonsterParams()
                .WithBreed("CleaningDroid")
                .AtPosition(new Point(0, 2))
                .WithState(MonsterState.Hunting);
            
            _gameWorld.SpawnMonster(spawnMonsterParams1);
            _gameWorld.SpawnMonster(spawnMonsterParams2);
            
            var monster1 = spawnMonsterParams1.Result;
            var monster2 = spawnMonsterParams2.Result;

            monster1.Health = 1;
            
            _gameWorld.TestResetFieldOfView();
            monster1.ResetFieldOfViewAndSeenTiles();
            monster2.ResetFieldOfViewAndSeenTiles();
            var result = monster2.NextTurn(_gameWorld.CommandFactory).ToList();

            // Act
            var lineAttackCommand = (LineAttackCommand)result[0]; 
            lineAttackCommand.Execute();

            foreach (var command in lineAttackCommand.CommandResult.SubsequentCommands)
            {
                command.Execute();
            }
            
            // Assert
            var attackCommand = result[0] as LineAttackCommand;
            Assert.IsNotNull(attackCommand);
            Assert.AreEqual(2, attackCommand.Targets.Count);
            Assert.AreSame(monster1, attackCommand.Targets[0]);
            Assert.AreSame(_gameWorld.Player, attackCommand.Targets[1]);
            Assert.AreSame(monster2, attackCommand.Source);
            Assert.IsTrue(monster1.IsDead);
        }
        
        [TestMethod]
        public void Monsters_Should_Stop_Acting_When_Player_Dies()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);

            _gameWorld.Player.Position = new Point(0, 0);

            var monster1 = new SpawnMonsterParams()
                .WithBreed("Roach")
                .AtPosition(new Point(0, 1))
                .WithState(MonsterState.Hunting);

            _gameWorld.SpawnMonster(monster1);
            
            var monster2 = new SpawnMonsterParams()
                .WithBreed("Roach")
                .AtPosition(new Point(1, 0))
                .WithState(MonsterState.Hunting);
            
            _gameWorld.SpawnMonster(monster2);
            
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
            var wallPosition1 = new Point(1, 1);
            var wallPosition2 = new Point(2, 2);

            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition1, wallPosition2 });
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator);

            _gameWorld.Player.Position = new Point(3, 3);

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
            var wallPosition = new Point(1, 1);

            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, new[] { wallPosition });
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld, mapGenerator);

            _gameWorld.Player.Position = new Point(3, 3);

            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("TeslaTurret").AtPosition(wallPosition));

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
            Assert.AreEqual(wallPosition, lightningAttackCommand.Path[0]);
            Assert.AreEqual(new Point(2, 2), lightningAttackCommand.Path[1]);
            Assert.AreEqual(_gameWorld.Player.Position, lightningAttackCommand.Path[2]);
        }
    }
}
