namespace MarsUndiscovered.Components
{
    public class ShipRepairParts : VictoryItemType
    {
        public override string Name => nameof(ShipRepairParts);

        public override string GetDescription(Item item, ItemDiscovery itemDiscovery, ItemTypeDiscovery itemTypeDiscovery, int quantity)
        {
            return "Ship Repair Parts";
        }

        public override string GetLongDescription(ItemTypeDiscovery itemTypeDiscovery)
        {
            return
                "These parts are able to repair the broken fuel injection system on the ship. Bring them back to the ship.";
        }
    }
}