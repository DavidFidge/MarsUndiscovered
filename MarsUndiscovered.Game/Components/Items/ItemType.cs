using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components
{
    public abstract class ItemType : GameObjectType
    {
        public static Dictionary<string, ItemType> ItemTypes;
        public static ShieldGenerator ShieldGenerator = new ShieldGenerator();
        public static ForcePush ForcePush = new ForcePush();
        public static MagnesiumPipe MagnesiumPipe = new MagnesiumPipe();
        public static IronSpike IronSpike = new IronSpike();
        public static LaserPistol LaserPistol = new LaserPistol();
        public static HealingBots HealingBots = new HealingBots();
        public static EnhancementBots EnhancementBots = new EnhancementBots();
        public static ShipRepairParts ShipRepairParts = new ShipRepairParts();

        public ItemType()
        {
            ForegroundColour = Color.Yellow;
            BackgroundColour = null;
        }

        public virtual bool GroupsInInventory { get; } = false;

        public virtual void RecalculateProperties(Item item)
        {
            item.GroupsInInventory = GroupsInInventory;
        }

        static ItemType()
        {
            ItemTypes = new Dictionary<string, ItemType>();

            ItemTypes.Add(ForcePush.Name, ForcePush);
            ItemTypes.Add(ShieldGenerator.Name, ShieldGenerator);
            ItemTypes.Add(MagnesiumPipe.Name, MagnesiumPipe);
            ItemTypes.Add(IronSpike.Name, IronSpike);
            ItemTypes.Add(LaserPistol.Name, LaserPistol);
            ItemTypes.Add(HealingBots.Name, HealingBots);
            ItemTypes.Add(EnhancementBots.Name, EnhancementBots);
            ItemTypes.Add(ShipRepairParts.Name, ShipRepairParts);
        }

        public static ItemType GetItemType(string itemType)
        {
            if (ItemTypes.ContainsKey(itemType))
                return ItemTypes[itemType];

            return null;
        }

        public abstract string GetDescription(Item item, ItemDiscovery itemDiscovery, ItemTypeDiscovery itemTypeDiscovery, int quantity, bool includePrefix = true, bool includeStatus = true);

        public string GetHotBarDescription(Item item, ItemDiscovery itemDiscovery)
        {
            if (!itemDiscovery.IsEnchantLevelDiscovered)
                return "?";

            return GetEnchantText(item);
        }
        
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

        public abstract string GetLongDescription(Item item, ItemTypeDiscovery itemTypeDiscovery);
    }
}
