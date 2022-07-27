using System.Collections.Generic;

namespace MarsUndiscovered.Components
{
    public abstract class ItemType
    {
        public static Dictionary<string, ItemType> ItemTypes;
        public static ShieldGenerator ShieldGenerator = new ShieldGenerator();
        public static MagnesiumPipe MagnesiumPipe = new MagnesiumPipe();
        public static IronSpike IronSpike = new IronSpike();
        public static HealingBots HealingBots = new HealingBots();
        public static ShipRepairParts ShipRepairParts = new ShipRepairParts();

        public virtual bool GroupsInInventory { get; } = false;

        public abstract string Name { get; }

        public virtual void ApplyProperties(Item item)
        {
            item.GroupsInInventory = GroupsInInventory;
        }

        public virtual void ApplyEnchantmentLevel(Item item)
        {
            item.EnchantmentLevel = 0;
        }

        static ItemType()
        {
            ItemTypes = new Dictionary<string, ItemType>();

            ItemTypes.Add(nameof(ShieldGenerator), ShieldGenerator);
            ItemTypes.Add(nameof(MagnesiumPipe), MagnesiumPipe);
            ItemTypes.Add(nameof(IronSpike), IronSpike);
            ItemTypes.Add(nameof(HealingBots), HealingBots);
            ItemTypes.Add(nameof(ShipRepairParts), ShipRepairParts);
        }

        public static ItemType GetItemType(string itemType)
        {
            if (ItemTypes.ContainsKey(itemType))
                return ItemTypes[itemType];

            return null;
        }

        public abstract string GetDescription(Item item, ItemDiscovery itemDiscovery, ItemTypeDiscovery itemTypeDiscovery, int quantity);

        protected string GetEnchantText(Item item)
        {
            if (item.EnchantmentLevel >= 0)
                return $"+{item.EnchantmentLevel}";

            return $"{item.EnchantmentLevel}";
        }

        protected string GetQuantityText(int quantity, ItemTypeDiscovery itemTypeDiscovery)
        {
            if (quantity == 1)
                return itemTypeDiscovery.UndiscoveredNamePrefix;

            return quantity.ToString();
        }
    }
}