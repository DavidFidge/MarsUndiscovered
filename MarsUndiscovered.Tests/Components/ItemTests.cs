using FrigidRogue.TestInfrastructure;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

namespace MarsUndiscovered.Tests.Components
{
    [TestClass]
    public class ItemTests : BaseTest
    {
        [TestMethod]
        public void Should_Create_MagnesiumPipe()
        {
            // Act
            var item = new Item(Substitute.For<IGameWorld>(), 1)
                .WithItemType(ItemType.MagnesiumPipe);

            // Assert
            Assert.AreEqual(ItemType.MagnesiumPipe, item.ItemType);
            Assert.AreEqual(0, item.EnchantmentLevel);
            Assert.AreEqual(5, item.MeleeAttack.DamageRange.Min);
            Assert.AreEqual(9, item.MeleeAttack.DamageRange.Max);
            Assert.IsFalse(item.IsCharged);
            Assert.AreEqual(0, item.DamageShieldPercentage);
            Assert.AreEqual(0, item.CurrentRechargeDelay);
            Assert.AreEqual(0, item.TotalRechargeDelay);
        }

        [TestMethod]
        public void Should_Create_ShieldGenerator()
        {
            // Act
            var item = new Item(Substitute.For<IGameWorld>(), 1)
                .WithItemType(ItemType.ShieldGenerator);

            // Assert
            Assert.AreEqual(ItemType.ShieldGenerator, item.ItemType);
            Assert.AreEqual(0, item.EnchantmentLevel);
            Assert.IsNull(item.MeleeAttack);
            Assert.IsTrue(item.IsCharged);
            Assert.AreEqual(30, item.DamageShieldPercentage);
            Assert.AreEqual(300, item.CurrentRechargeDelay);
            Assert.AreEqual(300, item.TotalRechargeDelay);
        }

        [TestMethod]
        public void Should_Create_HealingBots()
        {
            // Act
            var item = new Item(Substitute.For<IGameWorld>(), 1)
                .WithItemType(ItemType.HealingBots);

            // Assert
            Assert.AreEqual(ItemType.HealingBots, item.ItemType);
            Assert.AreEqual(0, item.EnchantmentLevel);
            Assert.IsNull(item.MeleeAttack);
            Assert.IsFalse(item.IsCharged);
            Assert.AreEqual(100, item.HealPercentOfMax);
            Assert.AreEqual((int)(Actor.BaseHealth * 0.5), item.MaxHealthIncrease);
        }

        [TestMethod]
        public void CanGroupWith_Should_Return_If_Items_Can_Group()
        {
            // Act
            var item1 = new Item(Substitute.For<IGameWorld>(), 1).WithItemType(ItemType.HealingBots);
            var item2 = new Item(Substitute.For<IGameWorld>(), 2).WithItemType(ItemType.HealingBots);
            var item3 = new Item(Substitute.For<IGameWorld>(), 3).WithItemType(ItemType.MagnesiumPipe);
            var item4 = new Item(Substitute.For<IGameWorld>(), 4).WithItemType(ItemType.MagnesiumPipe);

            // Assert
            Assert.IsTrue(item1.CanGroupWith(item2));
            Assert.IsFalse(item3.CanGroupWith(item4));
            Assert.IsFalse(item1.CanGroupWith(item4));
        }

        [TestMethod]
        [DataRow(1, false, "A Blue NanoFlask")]
        [DataRow(2, false, "2 Blue NanoFlasks")]
        [DataRow(1, true, "A NanoFlask of Healing Bots")]
        [DataRow(2, true, "2 NanoFlasks of Healing Bots")]
        public void Should_Get_HealingBots_Description(int quantity, bool isDiscovered, string expectedResult)
        {
            var item = new Item(Substitute.For<IGameWorld>(), 1)
                .WithItemType(ItemType.HealingBots);

            // Act
            var result = item.GetDescription(new ItemTypeDiscovery("A Blue") { IsItemTypeDiscovered = isDiscovered }, quantity);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow(1, false, false, "A Shiny Gadget")]
        [DataRow(1, false, true, "A Shield Generator Gadget")]
        [DataRow(1, true, true, "A +1 Shield Generator Gadget")]
        [DataRow(0, true, true, "A +0 Shield Generator Gadget")]
        [DataRow(-1, true, true, "A -1 Shield Generator Gadget")]
        public void Should_Get_ShieldGenerator_Description(int enchantmentLevel, bool isEnchantDiscovered, bool isDiscovered, string expectedResult)
        {
            var item = new Item(Substitute.For<IGameWorld>(), 1)
                .WithItemType(ItemType.ShieldGenerator)
                .WithEnchantmentLevel(enchantmentLevel);

            item.ItemDiscovery.IsEnchantLevelDiscovered = isEnchantDiscovered;

            // Act
            var result = item.GetDescription(new ItemTypeDiscovery("A Shiny") { IsItemTypeDiscovered = isDiscovered }, 1);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow(1, false, true, "A Magnesium Pipe")]
        [DataRow(1, false, false, "An Unknown Weapon")]
        [DataRow(1, true, true, "A +1 Magnesium Pipe")]
        [DataRow(0, true, true, "A +0 Magnesium Pipe")]
        [DataRow(-1, true, true, "A -1 Magnesium Pipe")]
        public void Should_Get_MagnesiumPipe_Description(int enchantmentLevel, bool isEnchantDiscovered, bool isDiscovered, string expectedResult)
        {
            var item = new Item(Substitute.For<IGameWorld>(), 1)
                .WithItemType(ItemType.MagnesiumPipe)
                .WithEnchantmentLevel(enchantmentLevel);

            item.ItemDiscovery.IsEnchantLevelDiscovered = isEnchantDiscovered;

            // Act
            var result = item.GetDescription(new ItemTypeDiscovery("A Shiny") { IsItemTypeDiscovered = isDiscovered }, 1);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
