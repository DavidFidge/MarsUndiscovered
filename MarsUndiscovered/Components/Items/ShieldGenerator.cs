using System;

using GoRogue.Components;

namespace MarsUndiscovered.Components
{
    public class ShieldGenerator : Gadget
    {
        private int _damageShieldPercentage = 30;
        public override string Name => "Shield Generator";

        protected override int RechargeDelay => 300;

        public override void ApplyProperties(Item item)
        {
            item.DamageShieldPercentage = _damageShieldPercentage + (item.EnchantmentLevel * 10);

            if (item.DamageShieldPercentage <= 0)
                item.DamageShieldPercentage = 5;

            if (item.EnchantmentLevel <= 0)
            {
                item.TotalRechargeDelay = RechargeDelay + RechargeDelay * (int)(Math.Abs(item.EnchantmentLevel) * 0.1);
            }
            else
            {
                item.TotalRechargeDelay = (int)(RechargeDelay / (0.9d * Math.Pow(1.2, item.EnchantmentLevel)));
            }

            item.IsCharged = true;
            item.CurrentRechargeDelay = item.TotalRechargeDelay;
        }
    }
}