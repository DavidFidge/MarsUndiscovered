using System;

using GoRogue.Components;

namespace MarsUndiscovered.Components
{
    public abstract class Gadget : ItemType
    {
        protected abstract int RechargeDelay { get; }

        public override string GetDescription(Item item, ItemDiscovery itemDiscovery, ItemTypeDiscovery itemTypeDiscovery, int quantity)
        {
            if (!itemTypeDiscovery.IsItemTypeDiscovered)
            {
                var description = $"{GetQuantityText(quantity, itemTypeDiscovery)} {itemTypeDiscovery.UndiscoveredName} Gadget";

                return quantity > 1 ? $"{description}s" : description;
            }

            return null;
        }
    }
}