using System.Text;
using MonoGame.Extended;

namespace MarsUndiscovered.Game.Components
{
    public class MagnesiumPipe : Weapon
    {
        private Attack _meleeAttack = new Attack(new Range<int>(5, 9));

        public MagnesiumPipe()
        {
            CanConcuss = true;
        }

        public override void RecalculateProperties(Item item)
        {
            base.RecalculateProperties(item);
            
            item.MeleeAttack = (Attack)_meleeAttack.Clone();

            var minDamage = item.MeleeAttack.DamageRangeBase.Min + item.EnchantmentLevel * 2;
            var maxDamage = item.MeleeAttack.DamageRangeBase.Max + item.EnchantmentLevel * 2;

            item.MeleeAttack.DamageRange = new Range<int>(minDamage <= 0 ? 1 : minDamage, maxDamage <= 0 ? 1 : maxDamage);
        }
        
        public override string GetTypeDescription()
        {
            return "Magnesium Pipe";
        }

        public override string GetLongDescription(Item item, ItemTypeDiscovery itemTypeDiscovery)
        {
            var stringBuilder = new StringBuilder();
            
            stringBuilder.AppendLine(
                "Magnesium ores are abundant on Mars and thus magnesium alloys are commonly used for lightweight, strong building materials and tools. This magnesium pipe looks like it would be useful for piping fluids, but it is also very handy as a clubbing weapon.");
            
            stringBuilder.AppendLine();
            
            if (item.ItemDiscovery.IsEnchantLevelDiscovered)
            {
                stringBuilder.Append($"{item.GetEnchantmentLevelText()} this weapon will hit for {item.MeleeAttack.GetAttackText(item.MeleeAttack.DamageRange)}");
            }
            else
            {
                stringBuilder.Append($"{GetPropertiesUnknownText()} this weapon would hit for {item.MeleeAttack.GetAttackText(item.MeleeAttack.DamageRange)}.");
            }

            return stringBuilder.ToString();
        }
    }
}
