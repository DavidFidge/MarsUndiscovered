namespace MarsUndiscovered.Game.Components
{
    public class LaserAttack : ICloneable
    { 
        public int Damage { get; set; }

        public LaserAttack(int damage)
        {
            Damage = damage;
        }

        public object Clone()
        {
            var laserAttack = (LaserAttack)MemberwiseClone();

            return laserAttack;
        }
        
        public string GetAttackText(int damage)
        {
            return $"{{{{L_BLUE}}}}{damage} damage{{{{DEFAULT}}}}";
        }
    }
}
