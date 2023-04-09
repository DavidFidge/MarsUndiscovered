using MarsUndiscovered.Game.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Tests.Components
{
    [TestClass]
    public class InventoryTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void Should_Add_Item_To_Inventory_And_Assign_It_Letter_A()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            var item = _gameWorld.Items.Values.First();

            // Act
            _gameWorld.Inventory.Add(item);

            // Assert
            Assert.AreEqual(1, _gameWorld.Inventory.ItemKeyAssignments.Count);
            Assert.AreEqual(Keys.A, _gameWorld.Inventory.ItemKeyAssignments.First().Key);
            Assert.AreEqual(1, _gameWorld.Inventory.ItemKeyAssignments.First().Value.Count);
            Assert.AreEqual(item, _gameWorld.Inventory.ItemKeyAssignments.First().Value.First());

            Assert.AreEqual(1, _gameWorld.Inventory.Items.Count);
            Assert.AreEqual(item, _gameWorld.Inventory.Items.First());

            Assert.AreEqual(0, _gameWorld.Inventory.CallItem.Count);
            Assert.AreEqual(0, _gameWorld.Inventory.CallItemType.Count);
        }

        [TestMethod]
        public void Should_Not_Add_Duplicate_Items()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            var item = _gameWorld.Items.Values.First();

            // Act
            _gameWorld.Inventory.Add(item);
            _gameWorld.Inventory.Add(item);

            // Assert
            Assert.AreEqual(1, _gameWorld.Inventory.ItemKeyAssignments.Count);
            Assert.AreEqual(Keys.A, _gameWorld.Inventory.ItemKeyAssignments.First().Key);
            Assert.AreEqual(1, _gameWorld.Inventory.ItemKeyAssignments.First().Value.Count);
            Assert.AreEqual(item, _gameWorld.Inventory.ItemKeyAssignments.First().Value.First());

            Assert.AreEqual(1, _gameWorld.Inventory.Items.Count);
            Assert.AreEqual(item, _gameWorld.Inventory.Items.First());

            Assert.AreEqual(0, _gameWorld.Inventory.CallItem.Count);
            Assert.AreEqual(0, _gameWorld.Inventory.CallItemType.Count);
        }

        [TestMethod]
        public void Should_Remove_Item_From_Inventory()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            var item = _gameWorld.Items.Values.First();
            _gameWorld.Inventory.Add(item);

            // Act
            _gameWorld.Inventory.Remove(item);

            // Assert
            Assert.AreEqual(0, _gameWorld.Inventory.ItemKeyAssignments.Count);
            Assert.AreEqual(0, _gameWorld.Inventory.Items.Count);
            Assert.AreEqual(0, _gameWorld.Inventory.CallItem.Count);
            Assert.AreEqual(0, _gameWorld.Inventory.CallItemType.Count);
        }

        [TestMethod]
        public void Should_Add_Two_Non_Groupable_Items_To_Inventory()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            var item1 = _gameWorld.Items.Values.First();
            var item2 = _gameWorld.Items.Values.Skip(1).First();

            // Act
            _gameWorld.Inventory.Add(item1);
            _gameWorld.Inventory.Add(item2);

            // Assert
            Assert.AreEqual(2, _gameWorld.Inventory.ItemKeyAssignments.Count);
            Assert.AreEqual(Keys.A, _gameWorld.Inventory.ItemKeyAssignments.First().Key);
            Assert.AreEqual(1, _gameWorld.Inventory.ItemKeyAssignments.First().Value.Count);
            Assert.AreEqual(Keys.B, _gameWorld.Inventory.ItemKeyAssignments.Skip(1).First().Key);
            Assert.AreEqual(1, _gameWorld.Inventory.ItemKeyAssignments.Skip(1).First().Value.Count);
            Assert.AreEqual(item1, _gameWorld.Inventory.ItemKeyAssignments.First().Value.First());
            Assert.AreEqual(item2, _gameWorld.Inventory.ItemKeyAssignments.Skip(1).First().Value.First());

            Assert.AreEqual(2, _gameWorld.Inventory.Items.Count);
            Assert.AreEqual(item1, _gameWorld.Inventory.Items.First(i => i == item1));
            Assert.AreEqual(item2, _gameWorld.Inventory.Items.First(i => i == item2));

            Assert.AreEqual(0, _gameWorld.Inventory.CallItem.Count);
            Assert.AreEqual(0, _gameWorld.Inventory.CallItemType.Count);
        }

        [TestMethod]
        public void Should_Add_Two_Groupable_Items_To_Inventory()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots));
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots));
            var item1 = _gameWorld.Items.Values.First();
            var item2 = _gameWorld.Items.Values.Skip(1).First();

            // Act
            _gameWorld.Inventory.Add(item1);
            _gameWorld.Inventory.Add(item2);

            // Assert
            Assert.AreEqual(1, _gameWorld.Inventory.ItemKeyAssignments.Count);
            Assert.AreEqual(Keys.A, _gameWorld.Inventory.ItemKeyAssignments.First().Key);
            Assert.AreEqual(2, _gameWorld.Inventory.ItemKeyAssignments.First().Value.Count);
            Assert.AreEqual(item1, _gameWorld.Inventory.ItemKeyAssignments.First().Value.First(i => i == item1));
            Assert.AreEqual(item2, _gameWorld.Inventory.ItemKeyAssignments.First().Value.First(i => i == item2));

            Assert.AreEqual(2, _gameWorld.Inventory.Items.Count);
            Assert.AreEqual(item1, _gameWorld.Inventory.Items.First(i => i == item1));
            Assert.AreEqual(item2, _gameWorld.Inventory.Items.First(i => i == item2));

            Assert.AreEqual(0, _gameWorld.Inventory.CallItem.Count);
            Assert.AreEqual(0, _gameWorld.Inventory.CallItemType.Count);
        }

        [TestMethod]
        public void Should_Remove_Groupable_Item_From_Inventory()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots));
            var item = _gameWorld.Items.Values.First();
            _gameWorld.Inventory.Add(item);

            // Act
            _gameWorld.Inventory.Remove(item);

            // Assert
            Assert.AreEqual(0, _gameWorld.Inventory.ItemKeyAssignments.Count);
            Assert.AreEqual(0, _gameWorld.Inventory.Items.Count);
            Assert.AreEqual(0, _gameWorld.Inventory.CallItem.Count);
            Assert.AreEqual(0, _gameWorld.Inventory.CallItemType.Count);
        }

        [TestMethod]
        public void Should_Remove_Groupable_Item_When_Multiple_In_Group()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots));
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots));
            var item1 = _gameWorld.Items.Values.First();
            var item2 = _gameWorld.Items.Values.Skip(1).First();
            _gameWorld.Inventory.Add(item1);
            _gameWorld.Inventory.Add(item2);

            // Act
            _gameWorld.Inventory.Remove(item1);

            // Assert
            Assert.AreEqual(1, _gameWorld.Inventory.ItemKeyAssignments.Count);
            Assert.AreEqual(Keys.A, _gameWorld.Inventory.ItemKeyAssignments.First().Key);
            Assert.AreEqual(1, _gameWorld.Inventory.ItemKeyAssignments.First().Value.Count);
            Assert.AreEqual(item2, _gameWorld.Inventory.ItemKeyAssignments.First().Value.First(i => i == item2));

            Assert.AreEqual(1, _gameWorld.Inventory.Items.Count);
            Assert.AreEqual(item2, _gameWorld.Inventory.Items.First(i => i == item2));

            Assert.AreEqual(0, _gameWorld.Inventory.CallItem.Count);
            Assert.AreEqual(0, _gameWorld.Inventory.CallItemType.Count);
        }

        [TestMethod]
        public void Should_Get_Item_By_Key()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            var item = _gameWorld.Items.Values.First();
            _gameWorld.Inventory.Add(item);

            // Act
            var result = _gameWorld.Inventory.GetItemForKey(Keys.A);

            // Assert
            Assert.AreEqual(item, result);
        }


        [TestMethod]
        public void Should_Return_Null_If_Assignment_Does_Not_Exist()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            var item = _gameWorld.Items.Values.First();
            _gameWorld.Inventory.Add(item);

            // Act
            var result = _gameWorld.Inventory.GetItemForKey(Keys.B);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Should_Assign_New_Key_When_Destination_Key_Not_Assigned()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            var item = _gameWorld.Items.Values.First();
            _gameWorld.Inventory.Add(item);

            // Act
            _gameWorld.Inventory.AssignNewKey(item, Keys.B);

            // Assert
            Assert.AreEqual(1, _gameWorld.Inventory.ItemKeyAssignments.Count);
            Assert.AreEqual(Keys.B, _gameWorld.Inventory.ItemKeyAssignments.First().Key);
            Assert.AreEqual(1, _gameWorld.Inventory.ItemKeyAssignments.First().Value.Count);
            Assert.AreEqual(item, _gameWorld.Inventory.ItemKeyAssignments.First().Value.First());
        }

        [TestMethod]
        public void Should_Assign_New_Key_When_Destination_Key_Is_Already_Assigned()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            var item1 = _gameWorld.Items.Values.First();
            var item2 = _gameWorld.Items.Values.Skip(1).First();
            _gameWorld.Inventory.Add(item1);
            _gameWorld.Inventory.Add(item2);

            // Act
            _gameWorld.Inventory.AssignNewKey(item2, Keys.A);

            // Assert
            Assert.AreEqual(2, _gameWorld.Inventory.ItemKeyAssignments.Count);
            Assert.AreEqual(Keys.B, _gameWorld.Inventory.ItemKeyAssignments.First(i => i.Value.First() == item1).Key);
            Assert.AreEqual(Keys.A, _gameWorld.Inventory.ItemKeyAssignments.First(i => i.Value.First() == item2).Key);
        }

        [TestMethod]
        public void Should_Not_Assign_Non_Alpha_Key()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            var item = _gameWorld.Items.Values.First();
            _gameWorld.Inventory.Add(item);

            // Act
            _gameWorld.Inventory.AssignNewKey(item, Keys.D1);

            // Assert
            Assert.AreEqual(1, _gameWorld.Inventory.ItemKeyAssignments.Count);
            Assert.AreEqual(Keys.A, _gameWorld.Inventory.ItemKeyAssignments.First().Key);
        }

        [TestMethod]
        public void GetInventoryItems_Should_Return_InventoryItems_For_Each_Key()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots));
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots));
            var item1 = _gameWorld.Items.Values.First();
            var item2 = _gameWorld.Items.Values.Skip(1).First();
            var item3 = _gameWorld.Items.Values.Skip(2).First();

            _gameWorld.Inventory.Add(item1);
            _gameWorld.Inventory.Add(item2);
            _gameWorld.Inventory.Add(item3);

            _gameWorld.Inventory.ItemTypeDiscoveries[ItemType.HealingBots].IsItemTypeDiscovered = true;

            // Act
            var result = _gameWorld.Inventory.GetInventoryItems();

            // Assert
            var resultKeyA = result.First(r => r.Key == Keys.A);
            Assert.AreEqual(item1.ItemType, resultKeyA.ItemType);
            Assert.AreEqual("a)", resultKeyA.KeyDescription);
            Assert.AreEqual("A Magnesium Pipe", resultKeyA.ItemDescription);

            var resultKeyB = result.First(r => r.Key == Keys.B);
            Assert.AreEqual(item2.ItemType, resultKeyB.ItemType);
            Assert.AreEqual("b)", resultKeyB.KeyDescription);
            Assert.AreEqual("2 NanoFlasks of Healing Bots", resultKeyB.ItemDescription);
        }

        [TestMethod]
        public void Should_Equip_Weapon()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            var item = _gameWorld.Items.Values.First();
            _gameWorld.Inventory.Add(item);

            // Act
            _gameWorld.Inventory.Equip(item);

            // Assert
            Assert.AreEqual(item, _gameWorld.Inventory.EquippedWeapon);
            Assert.IsTrue(_gameWorld.Inventory.IsEquipped(item));
        }

        [TestMethod]
        public void Should_Unequip_Weapon()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            var item = _gameWorld.Items.Values.First();
            _gameWorld.Inventory.Add(item);
            _gameWorld.Inventory.Equip(item);

            // Act
            _gameWorld.Inventory.Unequip(item);

            // Assert
            Assert.IsNull(_gameWorld.Inventory.EquippedWeapon);
            Assert.IsFalse(_gameWorld.Inventory.IsEquipped(item));
        }

        [TestMethod]
        public void Removing_An_Item_From_Inventory_Should_Also_Unequip_Weapon()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            var item = _gameWorld.Items.Values.First();
            _gameWorld.Inventory.Add(item);
            _gameWorld.Inventory.Equip(item);

            // Act
            _gameWorld.Inventory.Remove(item);

            // Assert
            Assert.IsNull(_gameWorld.Inventory.EquippedWeapon);
            Assert.IsFalse(_gameWorld.Inventory.IsEquipped(item));
        }

        [TestMethod]
        public void CanEquip_Weapon_Should_Return_True()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            var item = _gameWorld.Items.Values.First();
            _gameWorld.Inventory.Add(item);

            // Act
            var result = _gameWorld.Inventory.CanEquip(item);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanEquip_NonEquippable_ItemType_Should_Return_False()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots));
            var item = _gameWorld.Items.Values.First();
            _gameWorld.Inventory.Add(item);

            // Act
            var result = _gameWorld.Inventory.CanEquip(item);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanEquip_Weapon_Should_Return_False_If_Already_Equipped()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            var item = _gameWorld.Items.Values.First();
            _gameWorld.Inventory.Add(item);
            _gameWorld.Inventory.Equip(item);

            // Act
            var result = _gameWorld.Inventory.CanEquip(item);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanUnequip_Weapon_Should_Return_True()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            var item = _gameWorld.Items.Values.First();
            _gameWorld.Inventory.Add(item);
            _gameWorld.Inventory.Equip(item);

            // Act
            var result = _gameWorld.Inventory.CanUnequip(item);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanUnequip_Weapon_Should_Return_False_When_Not_Equipped()
        {
            // Arrange
            NewGameWithCustomMapNoMonstersNoItems();
            _gameWorld.SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe));
            var item = _gameWorld.Items.Values.First();
            _gameWorld.Inventory.Add(item);

            // Act
            var result = _gameWorld.Inventory.CanUnequip(item);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
