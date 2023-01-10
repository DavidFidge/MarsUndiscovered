using GoRogue.Random;
using MonoGame.Extended;

namespace MarsUndiscovered.Components
{
    public class Attack : ICloneable
    {
        public Range<int> DamageRangeBase { get; set; }
        public Range<int> DamageRange { get; set; }

        public Attack(Range<int> damageRangeBase)
        {
            DamageRangeBase = damageRangeBase;
            DamageRange = damageRangeBase;
        }

        public int Roll()
        {
            return GlobalRandom.DefaultRNG.NextInt(DamageRange.Min, DamageRange.Max);
        }

        public void SetPowerLevel(int powerLevel)
        {
            var minDamage = DamageRangeBase.Min + powerLevel;
            var maxDamage = DamageRangeBase.Max + powerLevel;

            DamageRange = new Range<int>(minDamage <= 0 ? 1 : minDamage, maxDamage <= 0 ? 1 : maxDamage);
        }

        public object Clone()
        {
            var attack = (Attack)MemberwiseClone();

            return attack;
        }
    }
}
