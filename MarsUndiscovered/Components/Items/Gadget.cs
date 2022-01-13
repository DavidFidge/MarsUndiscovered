using System;

using GoRogue.Components;

namespace MarsUndiscovered.Components
{
    public abstract class Gadget : ItemType
    {
        protected abstract int RechargeDelay { get; }
    }
}