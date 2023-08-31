using System.Text;

namespace MarsUndiscovered.Game.Components
{
    public class ShieldGenerator : Gadget
    {
        private int _damageShieldPercentage = 30;
        public override string Name => nameof(ShieldGenerator);

        public override string GetDescription(Item item, ItemDiscovery itemDiscovery, ItemTypeDiscovery itemTypeDiscovery, int quantity, bool includePrefix = true, bool includeStatus = true)
        {
            if (!itemTypeDiscovery.IsItemTypeDiscovered)
                return base.GetDescription(item, itemDiscovery, itemTypeDiscovery, quantity, includePrefix, includeStatus);

            var status = includeStatus ? $" ({item.GetRechargeText()})" : "";
            
            if (!itemDiscovery.IsEnchantLevelDiscovered)
                return $"{(includePrefix ? "A " : "")}{GetTypeDescription()}{status}";

            return $"{(includePrefix ? "A " : "")}{GetEnchantText(item)} {GetTypeDescription()}{status}";
        }

        public override string GetTypeDescription()
        {
            return $"Shield Generator {GetAbstractTypeName()}";
        }

        protected override int RechargeDelay => 300;

        public override void RecalculateProperties(Item item)
        {
            base.RecalculateProperties(item);

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
            item.CurrentRechargeDelay = 0;
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
                stringBuilder.AppendLine($"{item.GetEnchantmentLevelText()} this shield generator will give you a shield worth {{{{L_BLUE}}}}{item.DamageShieldPercentage}%{{{{DEFAULT}}}} of your maximum health.");
            }
            else
            {
                stringBuilder.AppendLine($"{GetPropertiesUnknownText()} the shield generator will act like a +1 device and give you a shield worth {{{{L_BLUE}}}}{_damageShieldPercentage}%{{{{DEFAULT}}}} of your maximum health.");
            }

            stringBuilder.AppendLine();

            stringBuilder.AppendLine(item.GetRechargeLongDescription());

            return stringBuilder.ToString();
        }
    }
}
