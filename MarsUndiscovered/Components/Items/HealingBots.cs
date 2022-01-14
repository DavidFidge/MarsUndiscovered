using System;

namespace MarsUndiscovered.Components
{
    public class HealingBots : NanoFlask
    {
        public override string Name => nameof(HealingBots);

        public override void ApplyProperties(Item item)
        {
            base.ApplyProperties(item);

            item.HealPercentOfMax = 100;
            item.MaxHealthIncrease = (int)(Actor.BaseHealth * 0.5);
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
    }
}