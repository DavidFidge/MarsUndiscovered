namespace MarsUndiscovered.Game.Components
{
    public class LightningAttack : ICloneable
    {
        public int BaseDamage { get; set; }
        public int Damage { get; set; }

        public LightningAttack(int damage)
        {
            Damage = damage;
            BaseDamage = damage;
        }

        public void SetPowerLevel(int powerLevel)
        {
            Damage = BaseDamage + powerLevel;
        }

        public object Clone()
        {
            var lightningAttack = (LightningAttack)MemberwiseClone();

            return lightningAttack;
        }
    }
}
