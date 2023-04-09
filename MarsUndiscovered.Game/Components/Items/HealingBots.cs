using System.Text;

namespace MarsUndiscovered.Game.Components
{
    public class HealingBots : NanoFlask
    {
        private readonly int _healthIncreasePercent = 25;
        public override string Name => nameof(HealingBots);

        public override void ApplyProperties(Item item)
        {
            base.ApplyProperties(item);

            item.HealPercentOfMax = 100;
            item.MaxHealthIncrease = ((Player.BaseHealth * _healthIncreasePercent) / 100);
        }

        public override string GetDescription(Item item, ItemDiscovery itemDiscovery,
            ItemTypeDiscovery itemTypeDiscovery, int quantity, bool includePrefix = true, bool includeStatus = true)
        {
            if (!itemTypeDiscovery.IsItemTypeDiscovered)
                return base.GetDescription(item, itemDiscovery, itemTypeDiscovery, quantity, includePrefix);

            if (quantity > 1)
                return $"{(includePrefix ? $"{quantity.ToString()} " : "")}{GetAbstractTypeDescription()}{(includePrefix ? "s" : "")} of Healing Bots";

            return $"{(includePrefix ? "A " : "")}{GetTypeDescription()}";
        }

        public override string GetTypeDescription()
        {
            return $"{GetAbstractTypeDescription()} of Healing Bots";
        }

        public override string GetLongDescription(Item item, ItemTypeDiscovery itemTypeDiscovery)
        {
            var baseLongDescription = base.GetLongDescription(item, itemTypeDiscovery);

            if (!String.IsNullOrEmpty(baseLongDescription))
                return baseLongDescription;
            
            var stringBuilder = new StringBuilder();
            
            stringBuilder.AppendLine(
                $"The contents of this flask must be poured onto an organic object. The nanobots inside will then slip inside under the skin of that organic and enter the blood stream. They will then repair any damaged tissue and break down any foreign object. They will also repair and improve broken DNA, making the organic even healthier than before.");
            
            stringBuilder.AppendLine();

            stringBuilder.Append(
                $"The target is healed to full health, loses all negative effects and gains an extra {{{{L_BLUE}}}}{_healthIncreasePercent}%{{{{DEFAULT}}}} of base health.");

            return stringBuilder.ToString();
        }
    }
}
