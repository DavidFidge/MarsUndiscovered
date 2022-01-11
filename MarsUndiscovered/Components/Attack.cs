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
    }
}