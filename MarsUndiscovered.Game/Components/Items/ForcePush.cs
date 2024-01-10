using System.Text;

namespace MarsUndiscovered.Game.Components
{
    public class ForcePush : Gadget
    {
        private int _defaultPushDistance = 2;

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
            return $"Force Push {GetAbstractTypeName()}";
        }

        protected override int RechargeDelay => 100;

        public override void RecalculateProperties(Item item)
        {
            base.RecalculateProperties(item);

            item.PushPullDistance = _defaultPushDistance + item.EnchantmentLevel;

            if (item.PushPullDistance <= 0)
                item.PushPullDistance = 0;

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
            
            stringBuilder.AppendLine("When activated, this gadget generates a force field around the wearer. The force field is then ejected in all directions outward at light speed, repelling anything in its path.");
            
            stringBuilder.AppendLine();

            if (itemTypeDiscovery.IsItemTypeDiscovered)
            {
                stringBuilder.AppendLine($"{item.GetEnchantmentLevelText()} this force push gadget will push all enemies up to {{{{L_BLUE}}}}{item.PushPullDistance}{{{{DEFAULT}}}} squares away from you.");
            }
            else
            {
                stringBuilder.AppendLine($"{GetPropertiesUnknownText()} the shield generator will act like a +0 device push all enemies up to {{{{L_BLUE}}}}{_defaultPushDistance}{{{{DEFAULT}}}} away from you.");
            }

            stringBuilder.AppendLine();

            stringBuilder.AppendLine(item.GetRechargeLongDescription());

            return stringBuilder.ToString();
        }
    }
}
