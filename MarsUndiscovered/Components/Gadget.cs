using System;

using GoRogue.Components;

namespace MarsUndiscovered.Components
{
    public class Gadget : Item
    {
        public Gadget(bool isWalkable = true, bool isTransparent = true, Func<uint> idGenerator = null, IComponentCollection customComponentCollection = null) : base(isWalkable, isTransparent, idGenerator, customComponentCollection)
        {
        }
    }
}