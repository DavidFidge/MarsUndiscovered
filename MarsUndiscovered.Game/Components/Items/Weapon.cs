namespace MarsUndiscovered.Game.Components
{
    public abstract class Weapon : ItemType
    {
        public override char AsciiCharacter => (char)0x18;

        public override string GetAbstractTypeName()
        {
            return nameof(Weapon);
        }
        
        public override string GetDescription(Item item, ItemDiscovery itemDiscovery,
            ItemTypeDiscovery itemTypeDiscovery, int quantity, bool includePrefix = true, bool includeStatus = true)
        {
            var status = includeStatus && item.IsEquipped ? " (equipped)" : "";
            
            if (itemTypeDiscovery is { IsItemTypeDiscovered: false })
                return $"{(includePrefix ? "An " : "")}Unknown {GetAbstractTypeName()}{status}";
               
            if (!itemDiscovery.IsEnchantLevelDiscovered)
                return $"{(includePrefix ? "A " : "")}{GetTypeDescription()}{status}";

            return $"{(includePrefix ? "A " : "")}{GetEnchantText(item)} {GetTypeDescription()}{status}";
        }

        public override string GetLongDescription(Item item, ItemTypeDiscovery itemTypeDiscovery)
        {
            return $"A {GetAbstractTypeName()}";
        }

        protected string GetPropertiesUnknownText()
        {
            return "The properties of this weapon are unknown. If unenchanted,";
        }
    }
}