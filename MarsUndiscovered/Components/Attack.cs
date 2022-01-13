using System;
using GoRogue.Random;
using MonoGame.Extended;

namespace MarsUndiscovered.Components
{
    public class Attack
    {
        public Range<int> DamageRange { get; private set; }

        public Attack(Range<int> damageRange)
        {
            DamageRange = damageRange;
        }

        public int Roll()
        {
            return GlobalRandom.DefaultRNG.Next(DamageRange.Min, DamageRange.Max);
        }

        public Attack Create(int powerLevel)
        {
            var minDamage = DamageRange.Min + powerLevel;
            var maxDamage = DamageRange.Max + powerLevel;

            var damageRange = new Range<int>(minDamage <= 0 ? 1 : minDamage, maxDamage <= 0 ? 1 : maxDamage);

            return new Attack(damageRange);
        }
    }
}