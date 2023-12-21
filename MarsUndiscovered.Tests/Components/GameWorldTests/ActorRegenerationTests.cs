using MarsUndiscovered.Game.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class ActorRegenerationTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void Should_Increase_Residual_Regen()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);

            _gameWorld.Player.Health = 1;
            _gameWorld.Player.MaxHealth = 100;
            _gameWorld.Player.RegenRate = 0.002m;
            
            // Act
            var result = _gameWorld.TestNextTurn().ToList();
            
            // Assert
            Assert.AreEqual(1, _gameWorld.Player.Health);
            Assert.AreEqual(0.2m, _gameWorld.Player.ResidualRegen);
        }
        
        [TestMethod]
        public void Should_Increase_Health_When_Residual_Regen_Spills()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);

            _gameWorld.Player.Health = 1;
            _gameWorld.Player.MaxHealth = 100;
            _gameWorld.Player.RegenRate = 0.006m;
            
            // Act
            var result = _gameWorld.TestNextTurn().ToList();
            result = _gameWorld.TestNextTurn().ToList();
            
            // Assert
            Assert.AreEqual(2, _gameWorld.Player.Health);
            Assert.AreEqual(0.2m, _gameWorld.Player.ResidualRegen);
        }
        
        [TestMethod]
        public void Should_Increase_Health_When_Residual_Regen_Spills_For_Regen_Rates_Greater_Than_One()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);

            _gameWorld.Player.Health = 1;
            _gameWorld.Player.MaxHealth = 100;
            _gameWorld.Player.RegenRate = 0.056m;
            
            // Act
            var result = _gameWorld.TestNextTurn().ToList();
            
            // Assert
            Assert.AreEqual(6, _gameWorld.Player.Health);
            Assert.AreEqual(0.6m, _gameWorld.Player.ResidualRegen);
        }
        
        [TestMethod]
        public void Residual_Should_Clear_When_Health_Increases_To_Full()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);

            _gameWorld.Player.Health = 99;
            _gameWorld.Player.MaxHealth = 100;
            _gameWorld.Player.RegenRate = 0.056m;
            
            // Act
            var result = _gameWorld.TestNextTurn().ToList();
            
            // Assert
            Assert.AreEqual(100, _gameWorld.Player.Health);
            Assert.AreEqual(0m, _gameWorld.Player.ResidualRegen);
        }     
        
        [TestMethod]
        public void Residual_Should_Clear_When_Health_Increases_To_Full_When_Residual_Exists()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);

            _gameWorld.Player.Health = 94;
            _gameWorld.Player.MaxHealth = 100;
            _gameWorld.Player.RegenRate = 0.056m;
            
            // Act
            var result = _gameWorld.TestNextTurn().ToList();
            result = _gameWorld.TestNextTurn().ToList();
            
            // Assert
            Assert.AreEqual(100, _gameWorld.Player.Health);
            Assert.AreEqual(0m, _gameWorld.Player.ResidualRegen);
        }     
        
        [TestMethod]
        public void Residual_Should_Not_Increase_When_Health_Is_Full()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);

            _gameWorld.Player.Health = 100;
            _gameWorld.Player.MaxHealth = 100;
            _gameWorld.Player.RegenRate = 0.056m;
            
            // Act
            var result = _gameWorld.TestNextTurn().ToList();
            
            // Assert
            Assert.AreEqual(100, _gameWorld.Player.Health);
            Assert.AreEqual(0m, _gameWorld.Player.ResidualRegen);
        }    
        
        [TestMethod]
        public void Should_Increase_Residual_Regen_Monsters()
        {
            // Arrange
            NewGameWithTestLevelGenerator(_gameWorld);
            
            _gameWorld.SpawnMonster(new SpawnMonsterParams().WithBreed("Roach").AtPosition(new Point(5, 0)));

            var monster = _gameWorld.Monsters.First().Value;
            monster.Health = 1;
            monster.MaxHealth = 100;
            monster.RegenRate = 0.056m;
            
            // Act
            var result = _gameWorld.TestNextTurn().ToList();
            
            // Assert
            Assert.AreEqual(6, monster.Health);
            Assert.AreEqual(0.6m, monster.ResidualRegen);
        }
    }
}