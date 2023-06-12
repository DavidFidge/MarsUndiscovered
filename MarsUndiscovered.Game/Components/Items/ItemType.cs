using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Game.Components
{
    public abstract class ItemType : GameObjectType
    {
        public static Dictionary<string, ItemType> ItemTypes;
        public static ShieldGenerator ShieldGenerator = new ShieldGenerator();
        public static MagnesiumPipe MagnesiumPipe = new MagnesiumPipe();
        public static IronSpike IronSpike = new IronSpike();
        public static HealingBots HealingBots = new HealingBots();
        public static ShipRepairParts ShipRepairParts = new ShipRepairParts();

        private Color _foregroundColour = Color.Yellow;
        private Color? _backgroundColour = null;

        public override string Name
        {
            get => GetType().Name;
            set { }
        }

        public override Color ForegroundColour
        {
            get => _foregroundColour;
            set => _foregroundColour = value;
        }

        public override Color? BackgroundColour
        {
            get => _backgroundColour;
            set => _backgroundColour = value;
        }

        public virtual bool GroupsInInventory { get; } = false;

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

            ItemTypes.Add(ShieldGenerator.Name, ShieldGenerator);
            ItemTypes.Add(MagnesiumPipe.Name, MagnesiumPipe);
            ItemTypes.Add(IronSpike.Name, IronSpike);
            ItemTypes.Add(HealingBots.Name, HealingBots);
            ItemTypes.Add(ShipRepairParts.Name, ShipRepairParts);
        }

        public static ItemType GetItemType(string itemType)
        {
            if (ItemTypes.ContainsKey(itemType))
                return ItemTypes[itemType];

            return null;
        }

        public abstract string GetDescription(Item item, ItemDiscovery itemDiscovery, ItemTypeDiscovery itemTypeDiscovery, int quantity, bool includePrefix = true, bool includeStatus = true);
        public abstract string GetTypeDescription();
        public abstract string GetAbstractTypeName();

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
