using System;

using GoRogue.Components;

namespace MarsUndiscovered.Components
{
    public abstract class NanoFlask : ItemType
    {
        public override bool GroupsInInventory => true;

        public override string GetDescription(Item item, ItemDiscovery itemDiscovery, ItemTypeDiscovery itemTypeDiscovery, int quantity)
        {
            if (!itemTypeDiscovery.IsItemTypeDiscovered)
            {
                var description = $"{GetQuantityText(quantity, itemTypeDiscovery)} {itemTypeDiscovery.UndiscoveredName} NanoFlask";

                return quantity > 1 ? $"{description}s" : description;
            }

            return null;
        }
    }
}