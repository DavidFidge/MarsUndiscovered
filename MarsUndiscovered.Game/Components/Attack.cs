using GoRogue.Random;
using MonoGame.Extended;

namespace MarsUndiscovered.Game.Components
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

        public object Clone()
        {
            var attack = (Attack)MemberwiseClone();

            return attack;
        }

        public string GetAttackText(Range<int> range)
        {
            return $"{{{{L_BLUE}}}}{range.Min}-{range.Max} damage{{{{DEFAULT}}}}";
        }
    }
}
