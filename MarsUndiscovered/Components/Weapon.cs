using System;

using GoRogue.Components;

namespace MarsUndiscovered.Components
{
    public class Weapon : Item
    {
        public Weapon(int layer, bool isWalkable = true, bool isTransparent = true, Func<uint> idGenerator = null, IComponentCollection customComponentCollection = null) : base(layer, isWalkable, isTransparent, idGenerator, customComponentCollection)
        {
        }
    }
}