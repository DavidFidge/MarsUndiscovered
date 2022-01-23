using AutoMapper;

using FrigidRogue.MonoGame.Core.Interfaces.Components;

using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class Item : MarsGameObject, IMementoState<ItemSaveData>
    {
        public ItemType ItemType { get; set; }
        public int EnchantmentLevel { get; set; }
        public int CurrentRechargeDelay { get; set; }
        public bool IsCharged { get; set; }

        public Attack MeleeAttack { get; set; }
        public int DamageShieldPercentage { get; set; }
        public int TotalRechargeDelay { get; set; }
        public int HealPercentOfMax { get; set; }
        public int MaxHealthIncrease { get; set; }
        public bool GroupsInInventory { get; set; }
        public bool HasBeenDropped { get; set; }
        public ItemDiscovery ItemDiscovery { get; set; } = new ItemDiscovery();

        public Item(uint id) : base(Constants.ItemLayer, true, true, () => id)
        {
        }

        public Item WithItemType(ItemType itemType)
        {
            ItemType = itemType;
            ItemType.ApplyEnchantmentLevel(this);
            ItemType.ApplyProperties(this);

            return this;
        }

        public Item WithEnchantmentLevel(int enchantmentLevel)
        {
            EnchantmentLevel = enchantmentLevel;

            return this;
        }

        public IMemento<ItemSaveData> GetSaveState(IMapper mapper)
        {
            return CreateWithAutoMapper<ItemSaveData>(mapper);
        }

        public void SetLoadState(IMemento<ItemSaveData> memento, IMapper mapper)
        {
            SetWithAutoMapper<ItemSaveData>(memento, mapper);
        }

        public bool CanGroupWith(Item item)
        {
            return GroupsInInventory && item.ItemType == ItemType;
        }

        public string GetDescription(ItemTypeDiscovery itemTypeDiscovery, int quantity)
        {
            return ItemType.GetDescription(this, ItemDiscovery, itemTypeDiscovery, quantity);
        }

        public string GetLongDescription(ItemTypeDiscovery itemTypeDiscovery)
        {
            return "Placeholder for long description";
        }
    }
}