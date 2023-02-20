using FrigidRogue.MonoGame.Core.Extensions;
using MarsUndiscovered.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class RadioCommsTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void GetNewRadioCommsItems_Should_Return_Populated_Objects()
        {
            // Arrange
            _gameWorld.NewGame();

            // Act
            var result = _gameWorld.GetNewRadioCommsItems();

            // Assert
            Assert.IsTrue(result.First().Message.StartsWith("Welcome to Mars captain!"));
            Assert.AreEqual(RadioComms.ShipAiSource, result.First().Source);
            Assert.AreSame(_gameWorld.Ships.First().Value, result.First().GameObject);
            
            Assert.IsTrue(result.Skip(1).First().Message.StartsWith("There's no communications signals"));
            Assert.AreEqual(RadioComms.ShipAiSource, result.Skip(1).First().Source);
            Assert.AreSame(_gameWorld.Ships.First().Value, result.Skip(1).First().GameObject, "Result of second item should match the FIRST ship item in the game world");
        }
        
        [TestMethod]
        public void GetNewRadioCommsItems_Should_Not_Return_Non_New_Objects()
        {
            // Arrange
            _gameWorld.NewGame();

            // Act
            _gameWorld.GetNewRadioCommsItems();
            var result = _gameWorld.GetNewRadioCommsItems();

            // Assert
            Assert.IsTrue(result.IsEmpty());
        }
        
        [TestMethod]
        public void RadioComms_Also_Inserts_Into_MessageLog()
        {
            // Arrange
            _gameWorld.NewGame();

            // Act
            var result = _gameWorld.GetMessagesSince(0);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.First().StartsWith(RadioComms.ShipAiSource));
            Assert.IsTrue(result.Last().StartsWith(RadioComms.ShipAiSource));
        }
    }
}