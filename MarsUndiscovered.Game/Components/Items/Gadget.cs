namespace MarsUndiscovered.Game.Components
{
    public abstract class Gadget : ItemType
    {
        protected abstract int RechargeDelay { get; }

        public Gadget()
        {
            AsciiCharacter = (char)237;
        }

        public override string GetDescription(Item item, ItemDiscovery itemDiscovery,
            ItemTypeDiscovery itemTypeDiscovery, int quantity, bool includePrefix = true, bool includeStatus = true)
        {
            if (itemTypeDiscovery is { IsItemTypeDiscovered: false })
                return
                    $"{GetQuantityText(1, itemTypeDiscovery)} {itemTypeDiscovery.UndiscoveredName} {GetAbstractTypeName()}";

            if (!itemDiscovery.IsEnchantLevelDiscovered)
                return $"{(includePrefix ? "A " : "")}{GetTypeDescription()}";

            return $"{(includePrefix ? "A " : "")}{GetEnchantText(item)} {GetTypeDescription()}";
        }

        public override string GetAbstractTypeName()
        {
            return nameof(Gadget);
        }

        public override string GetLongDescription(Item item, ItemTypeDiscovery itemTypeDiscovery)
        {
            if (!itemTypeDiscovery.IsItemTypeDiscovered)
            {
                return "This contraption has buttons, levers, flashing lights and screens all over it. Who knows what the effects will be if you fiddle around with it?";
            }

            return null;
        }
        
        protected string GetPropertiesUnknownText()
        {
            return "The properties of this gadget are unknown. While it remains undiscovered, ";
        }
    }
}