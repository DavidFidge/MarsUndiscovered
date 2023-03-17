namespace MarsUndiscovered.Components
{
    public abstract class Gadget : ItemType
    {
        protected abstract int RechargeDelay { get; }

        public override string GetDescription(Item item, ItemDiscovery itemDiscovery, ItemTypeDiscovery itemTypeDiscovery, int quantity, bool includePrefix = true, bool includeStatus = true)
        {
            if (!includePrefix)
                return $"{itemTypeDiscovery.UndiscoveredName} {GetAbstractTypeDescription()}";
            
            if (!itemTypeDiscovery.IsItemTypeDiscovered)
            {
                var description = $"{GetQuantityText(quantity, itemTypeDiscovery)} {itemTypeDiscovery.UndiscoveredName} {GetAbstractTypeDescription()}";

                return quantity > 1 ? $"{description}s" : description;
            }

            return null;
        }

        public override string GetAbstractTypeDescription()
        {
            return "Gadget";
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