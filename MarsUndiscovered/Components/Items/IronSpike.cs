using System.Text;
using MonoGame.Extended;

namespace MarsUndiscovered.Components
{
    public class IronSpike : Weapon
    {
        private Attack _lineAttack = new(new Range<int>(5, 9));

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

        public override string GetLongDescription(Item item, ItemTypeDiscovery itemTypeDiscovery)
        {
            var stringBuilder = new StringBuilder();
            
            stringBuilder.AppendLine("This large iron spike looks like it was once drove into the ground to support some infrastructure. Now it serves as an unwieldy spear. It is able to strike two enemies at once in the direction of attack.");
            
            stringBuilder.AppendLine();
            
            if (item.ItemDiscovery.IsEnchantLevelDiscovered)
            {
                stringBuilder.Append($"{item.GetEnchantmentLevelText()} this weapon will hit for {item.LineAttack.GetAttackText(item.LineAttack.DamageRange)} to all enemies within 2 squares of the direction of attack.");
            }
            else
            {
                stringBuilder.Append($"{GetPropertiesUnknownText()} this weapon would hit for {item.LineAttack.GetAttackText(item.LineAttack.DamageRange)} to all enemies within 2 squares of the direction of attack.");
            }

            return stringBuilder.ToString();
        }
    }
}
