using System.Linq;

using GoRogue.Random;

using MarsUndiscovered.Commands;
using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class SpawnGameObjectsTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void Should_Spawn_Monster()
        {
            // Arrange
            _gameWorld.NewGame();
            var currentMonsterCount = _gameWorld.Monsters.Count;

            // Act
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.Roach));

            // Assert
            Assert.AreEqual(currentMonsterCount + 1, _gameWorld.Monsters.Count);
        }

        [TestMethod]
        public void Should_Spawn_Items()
        {
            // Arrange
            _gameWorld.NewGame();
            var currentItemCount = _gameWorld.Items.Count;

            // Act
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));

            // Assert
            Assert.AreEqual(currentItemCount + 1, _gameWorld.Items.Count);
        }

        [TestMethod]
        public void Should_Spawn_Immobile_Monster_On_Wall()
        {
            // Arrange
            NewGameWithNoMonstersNoItems();

            var wallPosition = new Point(1, 1);
            _gameWorld.CreateWall(wallPosition);
            _gameWorld.Player.Position = new Point(3, 3);
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.TeslaCoil).AtPosition(wallPosition));

            // Act
            var monster = _gameWorld.CurrentMap.GetObjectAt<Monster>(wallPosition);

            // Assert
            Assert.AreSame(_gameWorld.Monsters.Values.First(), monster);
        }
    }
}