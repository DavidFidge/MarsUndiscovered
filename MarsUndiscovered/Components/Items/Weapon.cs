namespace MarsUndiscovered.Components
{
    public abstract class Weapon : ItemType
    {
        public override string GetDescription(Item item, ItemDiscovery itemDiscovery, ItemTypeDiscovery itemTypeDiscovery, int quantity)
        {
            if (itemTypeDiscovery != null && !itemTypeDiscovery.IsItemTypeDiscovered)
                return $"An Unknown Weapon";

            return null;
        }

        public override string GetLongDescription(Item item, ItemTypeDiscovery itemTypeDiscovery)
        {
            return "A weapon";
        }

        protected string GetPropertiesUnknownText()
        {
            return "The properties of this weapon are unknown. If unenchanted,";
        }
    }
}