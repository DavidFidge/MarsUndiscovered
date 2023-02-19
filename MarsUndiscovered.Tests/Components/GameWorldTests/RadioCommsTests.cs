using MarsUndiscovered.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class RadioCommsTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        [DataRow(0, 2)]
        [DataRow(1, 1)]
        [DataRow(2, 0)]
        [DataRow(3, 0)]
        public void GetRadioCommsItemsSince_Should_Get_New_Game_Radio_Comms_Messages_Based_On_Count_Provided(int providedCount, int expectedResultCount)
        {
            // Arrange
            _gameWorld.NewGame();

            // Act
            var result = _gameWorld.GetRadioCommsItemsSince(providedCount);

            // Assert
            Assert.AreEqual(expectedResultCount, result.Count);
        }
        
        [TestMethod]
        public void GetRadioCommsItemsSince_Should_Return_Populated_Objects()
        {
            // Arrange
            _gameWorld.NewGame();

            // Act
            var result = _gameWorld.GetRadioCommsItemsSince(0);

            // Assert
            Assert.IsTrue(result.First().Message.StartsWith("Welcome to Mars captain!"));
            Assert.AreEqual(RadioComms.ShipAiSource, result.First().Source);
            Assert.AreSame(_gameWorld.Ships.First().Value, result.First().GameObject);
            
            Assert.IsTrue(result.Skip(1).First().Message.StartsWith("There's no communications signals"));
            Assert.AreEqual(RadioComms.ShipAiSource, result.Skip(1).First().Source);
            Assert.AreSame(_gameWorld.Ships.First().Value, result.Skip(1).First().GameObject, "Result of second item should match the FIRST ship item in the game world");
        }
    }
}