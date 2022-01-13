using System;

using GoRogue.Components;

namespace MarsUndiscovered.Components
{
    public abstract class NanoFlask : ItemType
    {
        public override bool GroupsInInventory => true;
    }
}