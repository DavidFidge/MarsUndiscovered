using System.Text;

namespace MarsUndiscovered.Game.Components
{
    public class LaserPistol : Weapon
    {
        private LaserAttack _laserAttack = new LaserAttack(100);

        public override void RecalculateProperties(Item item)
        {
            base.RecalculateProperties(item);

            item.LaserAttack = (LaserAttack)_laserAttack.Clone();

            item.LaserAttack.Damage = (int)(item.LaserAttack.Damage * (1 + (item.EnchantmentLevel * 0.1f)));
        }
        
        public override string GetTypeDescription()
        {
            return "Laser Pistol";
        }

        public override string GetLongDescription(Item item, ItemTypeDiscovery itemTypeDiscovery)
        {
            var stringBuilder = new StringBuilder();
            
            stringBuilder.AppendLine("This lightweight laser pistol is a favored personal protection weapon of the general populate on Mars. It emits a tight high-energy beam that sears through anything that it is pointed at.");
            
            stringBuilder.AppendLine();
            
            if (item.ItemDiscovery.IsEnchantLevelDiscovered)
            {
                stringBuilder.Append($"{item.GetEnchantmentLevelText()} this weapon will hit for {item.LaserAttack.GetAttackText(item.LaserAttack.Damage)} to all enemies that the beam passes through");
            }
            else
            {
                stringBuilder.Append($"{GetPropertiesUnknownText()} this weapon would hit for {item.LaserAttack.GetAttackText(item.LaserAttack.Damage)} to all enemies that the beam passes through");
            }

            return stringBuilder.ToString();
        }
    }
}
