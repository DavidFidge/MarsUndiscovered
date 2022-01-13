using System;

using GoRogue.Components;

namespace MarsUndiscovered.Components
{
    public class HealingBots : NanoFlask
    {
        public override void ApplyProperties(Item item)
        {
            item.HealPercentOfMax = 100;
            item.MaxHealthIncrease = (int)(Actor.BaseHealth * 0.5);
        }

        public override string Name => "Nanobots of Healing";
    }
}