using System;

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

            item.MeleeAttack = _meleeAttack.Create(item.EnchantmentLevel);
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
    }
}