using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components
{
    public abstract class Gadget : ItemType
    {
        private char _asciiCharacter = (char)237;

        public override char AsciiCharacter
        {
            get => _asciiCharacter;
            set => _asciiCharacter = value;
        }

        protected abstract int RechargeDelay { get; }

        public override string GetDescription(Item item, ItemDiscovery itemDiscovery, ItemTypeDiscovery itemTypeDiscovery, int quantity, bool includePrefix = true, bool includeStatus = true)
        {
            if (!includePrefix)
                return $"{itemTypeDiscovery.UndiscoveredName} {GetAbstractTypeName()}";
            
            if (!itemTypeDiscovery.IsItemTypeDiscovered)
            {
                var description = $"{GetQuantityText(quantity, itemTypeDiscovery)} {itemTypeDiscovery.UndiscoveredName} {GetAbstractTypeName()}";

                return quantity > 1 ? $"{description}s" : description;
            }

            return null;
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