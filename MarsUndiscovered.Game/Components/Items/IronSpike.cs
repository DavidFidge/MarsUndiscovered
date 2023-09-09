using System.Text;
using MonoGame.Extended;

namespace MarsUndiscovered.Game.Components
{
    public class IronSpike : Weapon
    {
        private Attack _lineAttack = new(new Range<int>(12, 16));

        public override string Name => nameof(IronSpike);

        public override void RecalculateProperties(Item item)
        {
            base.RecalculateProperties(item);

            item.LineAttack = (Attack)_lineAttack.Clone();

            var minDamage = item.LineAttack.DamageRangeBase.Min + item.EnchantmentLevel * 2;
            var maxDamage = item.LineAttack.DamageRangeBase.Max + item.EnchantmentLevel * 2;

            item.LineAttack.DamageRange = new Range<int>(minDamage <= 0 ? 1 : minDamage, maxDamage <= 0 ? 1 : maxDamage);
        }
        
        public override string GetTypeDescription()
        {
            return "Iron Spike";
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
