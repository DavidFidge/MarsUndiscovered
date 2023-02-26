using MonoGame.Extended;

namespace MarsUndiscovered.Components
{
    public class MagnesiumPipe : Weapon
    {
        private Attack _meleeAttack = new Attack(new Range<int>(5, 9));

        public override string Name => nameof(MagnesiumPipe);

        public override void ApplyProperties(Item item)
        {
            base.ApplyProperties(item);

            item.MeleeAttack = (Attack)_meleeAttack.Clone();
            item.MeleeAttack.SetPowerLevel(item.EnchantmentLevel);
        }

        public override string GetDescription(Item item, ItemDiscovery itemDiscovery, ItemTypeDiscovery itemTypeDiscovery, int quantity)
        {
            var baseDescription = base.GetDescription(item, itemDiscovery, itemTypeDiscovery, quantity);

            if (!String.IsNullOrEmpty(baseDescription))
                return baseDescription;

            if (!itemDiscovery.IsEnchantLevelDiscovered)
                return $"A Magnesium Pipe";

            return $"A {GetEnchantText(item)} Magnesium Pipe";
        }

        public override string GetLongDescription(ItemTypeDiscovery itemTypeDiscovery)
        {
            return
                "Magnesium ores are abundant on Mars and thus magnesium alloys are commonly used for lightweight, strong building materials and tools. This magnesium pipe looks like it would be useful for piping fluids, but it is also very handy as a clubbing weapon.";
        }
    }
}
