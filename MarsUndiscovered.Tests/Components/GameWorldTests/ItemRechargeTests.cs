using MarsUndiscovered.Game.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class ItemRechargeTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void When_Next_Turn_Occurs_Current_Recharge_Should_Not_Drop_Below_Zero()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
            
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator));
            var item = _gameWorld.Items.Values.Last();
            
            // Act
            var result = _gameWorld.TestNextTurn().ToList();
            
            // Assert
            Assert.AreEqual(0, item.CurrentRechargeDelay);
            Assert.AreEqual(0, _gameWorld.GetMessagesSince(0).Count(g => g.Contains(" has recharged")));
        }
        
        [TestMethod]
        public void Items_Should_Spawn_Fully_Charged()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
            
            // Act
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator));
            
            // Assert
            var item = _gameWorld.Items.Values.Last();
            Assert.AreEqual(0, item.CurrentRechargeDelay);
        }
        
        [TestMethod]
        public void ResetRechargeDelay_Should_Reset_Item_To_Uncharged()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator));
            var item = _gameWorld.Items.Values.Last();
 
            // Act
            item.ResetRechargeDelay();
            
            // Assert
            Assert.IsTrue(item.TotalRechargeDelay > 0);
            Assert.AreEqual(item.TotalRechargeDelay, item.CurrentRechargeDelay);
        }
        
        [TestMethod]
        public void FullRecharge_Should_Fully_Recharge_Item()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator));
            var item = _gameWorld.Items.Values.Last();
            item.ResetRechargeDelay();

            // Act
            item.FullRecharge();
            
            // Assert
            Assert.AreEqual(0, item.CurrentRechargeDelay);
        }
        
        [TestMethod]
        public void When_Next_Turn_Occurs_Item_Recharge_Delay_Should_Be_Decremented()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
            
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator));
            var item = _gameWorld.Items.Values.Last();
            item.ResetRechargeDelay();
            
            // Act
            var result = _gameWorld.TestNextTurn().ToList();
            
            // Assert
            Assert.AreEqual(item.TotalRechargeDelay - 1, item.CurrentRechargeDelay);
            Assert.AreEqual(0, _gameWorld.GetMessagesSince(0).Count(g => g.Contains(" has recharged")));
        }
        
        [TestMethod]
        public void When_Item_Has_Charged_A_Message_Is_Logged()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItemsNoExitsNoStructures(_gameWorld);
            
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator));
            var item = _gameWorld.Items.Values.Last();
            item.CurrentRechargeDelay = 1;
            
            // Act
            var result = _gameWorld.TestNextTurn().ToList();
            
            // Assert
            Assert.AreEqual(0, item.CurrentRechargeDelay);
            Assert.AreEqual(1, _gameWorld.GetMessagesSince(0).Count(g => g.Contains(" has recharged")));
        }
    }
}