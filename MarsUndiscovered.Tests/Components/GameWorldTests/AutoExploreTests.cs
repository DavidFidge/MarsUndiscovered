﻿using System.Linq;

using MarsUndiscovered.Components;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            NewGameWithCustomMapNoMonstersNoItemsNoExits();

            _gameWorld.Player.Position = new Point(0, 0);
            var wallPosition1 = new Point(1, 0);
            var wallPosition2 = new Point(1, 1);
            var wallPosition3 = new Point(1, 2);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.CreateWall(wallPosition2);
            _gameWorld.CreateWall(wallPosition3);

            _gameWorld.ResetFieldOfView();

            // Act
            var result = _gameWorld.AutoExploreRequest();

            // Assert
            Assert.AreEqual(new Point(0, 1), _gameWorld.Player.Position);

            Assert.AreEqual(4, result.Path.LengthWithStart);
            Assert.AreEqual(new Point(0, 1), result.Path.GetStepWithStart(0));
            Assert.AreEqual(new Point(0, 2), result.Path.GetStepWithStart(1));
            Assert.AreEqual(new Point(1, 3), result.Path.GetStepWithStart(2));
            Assert.AreEqual(new Point(2, 3), result.Path.GetStepWithStart(3));
        }

        [TestMethod]
        public void AutoExplore_Should_Prioritise_Picking_Up_Undropped_Items()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExits();

            _gameWorld.Player.Position = new Point(0, 0);
            var wallPosition1 = new Point(1, 0);
            var wallPosition2 = new Point(1, 1);
            var wallPosition3 = new Point(1, 2);
            var itemPosition = new Point(0, 5);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.CreateWall(wallPosition2);
            _gameWorld.CreateWall(wallPosition3);
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(itemPosition));

            _gameWorld.ResetFieldOfView();

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
            NewGameWithCustomMapNoMonstersNoItemsNoExits();

            _gameWorld.Player.Position = new Point(0, 0);
            var wallPosition1 = new Point(1, 0);
            var wallPosition2 = new Point(1, 1);
            var wallPosition3 = new Point(1, 2);
            var itemPosition = new Point(0, 5);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.CreateWall(wallPosition2);
            _gameWorld.CreateWall(wallPosition3);
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).AtPosition(itemPosition));
            _gameWorld.Items.Values.First().HasBeenDropped = true;

            _gameWorld.ResetFieldOfView();

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
            NewGameWithCustomMapNoMonstersNoItemsNoExits();

            _gameWorld.Player.Position = new Point(0, 0);
            var wallPosition1 = new Point(1, 0);
            var wallPosition2 = new Point(1, 1);
            var wallPosition3 = new Point(1, 2);
            var mapExitPosition = new Point(0, 5);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.CreateWall(wallPosition2);
            _gameWorld.CreateWall(wallPosition3);
            _gameWorld.SpawnMapExit(new SpawnMapExitParams().AtPosition(mapExitPosition).WithDirection(Direction.Down));
            _gameWorld.ResetFieldOfView();

            // Act
            var result1 = _gameWorld.AutoExploreRequest(true);
            var result2 = _gameWorld.AutoExploreRequest(true);
            var result3 = _gameWorld.AutoExploreRequest(true);
            var result4 = _gameWorld.AutoExploreRequest(true);

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
        }

        [TestMethod]
        public void AutoExplore_Should_Not_Go_To_Exit_When_Fallback_To_Map_Exit_Is_False()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExits();

            _gameWorld.Player.Position = new Point(0, 0);
            var wallPosition1 = new Point(1, 0);
            var wallPosition2 = new Point(1, 1);
            var wallPosition3 = new Point(1, 2);
            var mapExitPosition = new Point(0, 5);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.CreateWall(wallPosition2);
            _gameWorld.CreateWall(wallPosition3);
            _gameWorld.SpawnMapExit(new SpawnMapExitParams().AtPosition(mapExitPosition).WithDirection(Direction.Down));
            _gameWorld.ResetFieldOfView();

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
        public void AutoExplore_Should_Not_Move_If_Stuck_In_Walls()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExits();

            _gameWorld.Player.Position = new Point(0, 0);

            for (var y = 0; y < 2; y++)
            {
                for (var x = 0; x < 2; x++)
                {
                    var point = new Point(x, y);

                    if (point != new Point(0, 0))
                        _gameWorld.CreateWall(new Point(x, y));
                }
            }

            _gameWorld.ResetFieldOfView();

            // Act
            var result = _gameWorld.AutoExploreRequest();

            // Assert
            Assert.AreEqual(new Point(0, 0), _gameWorld.Player.Position);
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.Count());
        }

        [TestMethod]
        public void AutoExplore_Should_Not_Get_Stuck_On_Wall()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExits();

            _gameWorld.Player.Position = new Point(0, 0);

            for (var y = 1; y < 6; y++)
            {
                for (var x = 1; x < 6; x++)
                {
                    _gameWorld.CreateWall(new Point(x, y));
                }
            }

            _gameWorld.ResetFieldOfView();

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
            NewGameWithCustomMapNoMonstersNoItemsNoExits();

            var playerPosition = new Point(0, 0);

            _gameWorld.Player.Position = playerPosition;

            var wallPosition1 = new Point(1, 0);
            var wallPosition2 = new Point(1, 1);
            var wallPosition3 = new Point(1, 2);
            var wallPosition4 = new Point(0, 2);

            _gameWorld.CreateWall(wallPosition1);
            _gameWorld.CreateWall(wallPosition2);
            _gameWorld.CreateWall(wallPosition3);
            _gameWorld.CreateWall(wallPosition4);

            _gameWorld.ResetFieldOfView();

            // Act
            var result = _gameWorld.AutoExploreRequest();

            // Assert
            Assert.AreEqual(playerPosition, _gameWorld.Player.Position);
            Assert.AreEqual(0, _gameWorld.HistoricalCommands.Count());
            Assert.AreEqual(1, result.Path.LengthWithStart);
            Assert.AreEqual(playerPosition, result.Path.Start);
        }

        [TestMethod]
        public void AutoExplore_Should_Attack_Adjacent_Monsters_Before_Exploring()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExits();

            var playerPosition = new Point(0, 0);
            _gameWorld.Player.Position = playerPosition;

            for (var y = 1; y < 6; y++)
            {
                for (var x = 1; x < 6; x++)
                {
                    _gameWorld.CreateWall(new Point(x, y));
                }
            }

            var monsterPosition = new Point(1, 0);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach).AtPosition(monsterPosition));
            _gameWorld.ResetFieldOfView();

            // Act
            var result = _gameWorld.AutoExploreRequest();

            // Assert
            Assert.AreEqual(2, result.Path.LengthWithStart);
            Assert.AreEqual(playerPosition, result.Path.Start);
            Assert.AreEqual(monsterPosition, result.Path.End);
        }
    }
}