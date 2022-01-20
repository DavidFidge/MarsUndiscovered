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
    }
}