using System;
using GoRogue.Random;
using MonoGame.Extended;

namespace MarsUndiscovered.Components
{
    public class Attack
    {
        private readonly Range<int> _damageRange;

        public Attack(Range<int> damageRange)
        {
            _damageRange = damageRange;
        }

        public int Roll()
        {
            return GlobalRandom.DefaultRNG.Next(_damageRange.Min, _damageRange.Max);
        }
    }
}