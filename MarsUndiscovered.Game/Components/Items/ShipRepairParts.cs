namespace MarsUndiscovered.Game.Components
{
    public class ShipRepairParts : VictoryItemType
    {
        public override char AsciiCharacter => '&';

        public override string Name => nameof(ShipRepairParts);

        public override string GetDescription(Item item, ItemDiscovery itemDiscovery,
            ItemTypeDiscovery itemTypeDiscovery, int quantity, bool includePrefix = true, bool includeStatus = true)
        {
            return GetTypeDescription();
        }

        public override string GetTypeDescription()
        {
            return "Ship Repair Parts";
        }

        public override string GetAbstractTypeName()
        {
            return nameof(ShipRepairParts);
        }

        public override string GetLongDescription(Item item, ItemTypeDiscovery itemTypeDiscovery)
        {
            return
                "These parts are able to repair the broken fuel injection system on the ship. Bring them back to the ship.";
        }
    }
}