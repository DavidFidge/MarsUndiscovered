using System;

namespace MarsUndiscovered.Components
{
    public class ShipRepairParts : VictoryItemType
    {
        public override string Name => nameof(ShipRepairParts);

        public override string GetDescription(Item item, ItemDiscovery itemDiscovery, ItemTypeDiscovery itemTypeDiscovery, int quantity)
        {
            return "Ship Repair Parts";
        }
    }
}