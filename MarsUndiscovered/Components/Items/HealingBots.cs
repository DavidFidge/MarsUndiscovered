namespace MarsUndiscovered.Components
{
    public class HealingBots : NanoFlask
    {
        public override string Name => nameof(HealingBots);

        public override void ApplyProperties(Item item)
        {
            base.ApplyProperties(item);

            item.HealPercentOfMax = 100;
            item.MaxHealthIncrease = (int)(Player.BaseHealth * 0.5);
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

        public override string GetLongDescription(ItemTypeDiscovery itemTypeDiscovery)
        {
            return
                "The contents of this flask must be poured onto an organic object. The nanobots inside will then slip inside under the skin of that organic and enter the blood stream. They will then repair any damaged tissue and break down any foreign object. They will also repair and improve broken DNA, making the organic even healthier than before.";
        }
    }
}
