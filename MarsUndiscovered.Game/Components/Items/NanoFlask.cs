namespace MarsUndiscovered.Game.Components
{
    public abstract class NanoFlask : ItemType
    {
        public override char AsciiCharacter => (char)0x9a;

        public override bool GroupsInInventory => true;

        public override string GetDescription(Item item, ItemDiscovery itemDiscovery,
            ItemTypeDiscovery itemTypeDiscovery, int quantity, bool includePrefix = true, bool includeStatus = true)
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
            return nameof(NanoFlask);
        }

        public override string GetLongDescription(Item item, ItemTypeDiscovery itemTypeDiscovery)
        {
            if (!itemTypeDiscovery.IsItemTypeDiscovered)
            {
                return
                    "When you look closely into the flask you swear you can see nanobots swimming around. Who knows what the effects will be when poured on yourself or thrown at something?";
            }

            return null;
        }
    }
}