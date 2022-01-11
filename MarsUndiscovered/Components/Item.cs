using System;

using GoRogue.Components;

namespace MarsUndiscovered.Components
{
    public abstract class Item : MarsGameObject
    {
        public Item(Func<uint> idGenerator = null, IComponentCollection customComponentCollection = null) : base(2, true, isTransparent, idGenerator, customComponentCollection)
        {
        }
    }
}