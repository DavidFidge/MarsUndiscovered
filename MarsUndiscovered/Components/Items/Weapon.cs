namespace MarsUndiscovered.Components
{
    public abstract class Weapon : ItemType
    {
        public override string GetAbstractTypeDescription()
        {
            return "Weapon";
        }
        
        public override string GetDescription(Item item, ItemDiscovery itemDiscovery,
            ItemTypeDiscovery itemTypeDiscovery, int quantity, bool includePrefix = true, bool includeStatus = true)
        {
            if (itemTypeDiscovery is { IsItemTypeDiscovered: false })
                return $"{(includePrefix ? "An " : "")}Unknown {GetAbstractTypeDescription()}";
               
            if (!itemDiscovery.IsEnchantLevelDiscovered)
                return $"{(includePrefix ? "A " : "")}{GetTypeDescription()}";

            return $"{(includePrefix ? "A " : "")}{GetEnchantText(item)} {GetTypeDescription()}";
        }

        public override string GetLongDescription(Item item, ItemTypeDiscovery itemTypeDiscovery)
        {
            return $"A {GetAbstractTypeDescription()}";
        }

        protected string GetPropertiesUnknownText()
        {
            return "The properties of this weapon are unknown. If unenchanted,";
        }
    }
}