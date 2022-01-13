using System;
using System.Collections.Generic;
using System.Text;

using FrigidRogue.TestInfrastructure;

using MarsUndiscovered.Components;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarsUndiscovered.Tests.Components
{
    [TestClass]
    public class ItemTests : BaseTest
    {
        [TestMethod]
        public void Should_Create_MagnesiumPipe()
        {
            // Act
            var item = new Item(1)
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
            var item = new Item(1)
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
    }
}
