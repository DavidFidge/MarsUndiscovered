using System;

using GoRogue.Components;
using GoRogue.Random;

using MonoGame.Extended;

namespace MarsUndiscovered.Components
{
    public class MagnesiumPipe : Weapon
    {
        private Attack _meleeAttack = new Attack(new Range<int>(5, 9));
        public override string Name => "Magnesium Pipe";

        public override void ApplyProperties(Item item)
        {
            item.MeleeAttack = _meleeAttack.Create(item.EnchantmentLevel);
        }
    }
}