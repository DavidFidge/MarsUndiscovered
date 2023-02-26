using System.Text;

namespace MarsUndiscovered.Components
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

        public override string GetDescription(Item item, ItemDiscovery itemDiscovery, ItemTypeDiscovery itemTypeDiscovery, int quantity)
        {
            var baseDescription = base.GetDescription(item, itemDiscovery, itemTypeDiscovery, quantity);

            if (!String.IsNullOrEmpty(baseDescription))
                return baseDescription;

            if (quantity > 1)
                return $"{quantity} NanoFlasks of Healing Bots";

            return "A NanoFlask of Healing Bots";
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
