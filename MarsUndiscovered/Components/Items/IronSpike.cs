using MonoGame.Extended;

namespace MarsUndiscovered.Components
{
    public class IronSpike : Weapon
    {
        private Attack _lineAttack = new Attack(new Range<int>(5, 9));

        public override string Name => nameof(IronSpike);

        public override void ApplyProperties(Item item)
        {
            base.ApplyProperties(item);

            item.LineAttack = (Attack)_lineAttack.Clone();
            item.LineAttack.SetPowerLevel(item.EnchantmentLevel);
        }

        public override string GetDescription(Item item, ItemDiscovery itemDiscovery, ItemTypeDiscovery itemTypeDiscovery, int quantity)
        {
            var baseDescription = base.GetDescription(item, itemDiscovery, itemTypeDiscovery, quantity);

            if (!String.IsNullOrEmpty(baseDescription))
                return baseDescription;

            if (!itemDiscovery.IsEnchantLevelDiscovered)
                return $"An Iron Spike";

            return $"A {GetEnchantText(item)} Iron Spike";
        }
    }
}
