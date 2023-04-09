using System.Text;
using MonoGame.Extended;

namespace MarsUndiscovered.Game.Components
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
