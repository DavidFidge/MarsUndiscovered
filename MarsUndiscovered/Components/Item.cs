using System;

using GoRogue.Components;

namespace MarsUndiscovered.Components
{
    public class Item : MarsGameObject
    {
        public Item(uint id) : base(2, true, true, () => id)
        {
        }
    }
}