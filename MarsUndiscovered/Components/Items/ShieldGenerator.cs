using System.Text;

namespace MarsUndiscovered.Components
{
    public class ShieldGenerator : Gadget
    {
        private int _damageShieldPercentage = 30;
        public override string Name => nameof(ShieldGenerator);

        public override string GetDescription(Item item, ItemDiscovery itemDiscovery, ItemTypeDiscovery itemTypeDiscovery, int quantity)
        {
            var baseDescription = base.GetDescription(item, itemDiscovery, itemTypeDiscovery, quantity);

            if (!String.IsNullOrEmpty(baseDescription))
                return baseDescription;

            if (!itemDiscovery.IsEnchantLevelDiscovered)
                return $"A Shield Generator Gadget";

            return $"A {GetEnchantText(item)} Shield Generator Gadget";
        }

        protected override int RechargeDelay => 300;

        public override void ApplyProperties(Item item)
        {
            base.ApplyProperties(item);

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

        public override string GetLongDescription(Item item, ItemTypeDiscovery itemTypeDiscovery)
        {
            var baseLongDescription = base.GetLongDescription(item, itemTypeDiscovery);

            if (!String.IsNullOrEmpty(baseLongDescription))
                return baseLongDescription;
            
            var stringBuilder = new StringBuilder();
            
            stringBuilder.AppendLine("Strapped around the waist, this device emits a band of negative energy around the wearer. Any harmful object or particle enters into the space immediately surrounding the wearer, it immediately repels it, thus protecting the wearer from harm.");
            
            stringBuilder.AppendLine();

            if (itemTypeDiscovery.IsItemTypeDiscovered)
            {
                stringBuilder.Append($"{item.GetEnchantmentLevelText()} this shield generator will give you a shield worth {{{{L_BLUE}}}}{item.DamageShieldPercentage}%{{{{DEFAULT}}}} of your maximum health.");
            }
            else
            {
                stringBuilder.Append($"{GetPropertiesUnknownText()} the shield generator will act like a +1 device and give you a shield worth {{{{L_BLUE}}}}{_damageShieldPercentage}%{{{{DEFAULT}}}} of your maximum health.");
            }

            return stringBuilder.ToString();
        }
    }
}
