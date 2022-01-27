using AutoMapper;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

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

        public IMemento<ItemSaveData> GetSaveState()
        {
            var memento = new Memento<ItemSaveData>();

            base.PopulateSaveState(memento.State);

            memento.State.ItemTypeName = ItemType.Name;
            memento.State.EnchantmentLevel = EnchantmentLevel;
            memento.State.CurrentRechargeDelay = CurrentRechargeDelay;
            memento.State.IsCharged = IsCharged;
            memento.State.MeleeAttack = (Attack)MeleeAttack.Clone();
            memento.State.DamageShieldPercentage = DamageShieldPercentage;
            memento.State.TotalRechargeDelay = TotalRechargeDelay;
            memento.State.HealPercentOfMax = HealPercentOfMax;
            memento.State.MaxHealthIncrease = MaxHealthIncrease;
            memento.State.GroupsInInventory = GroupsInInventory;
            memento.State.HasBeenDropped = HasBeenDropped;
            memento.State.ItemDiscovery = (ItemDiscovery)ItemDiscovery.Clone();

            return memento;
        }

        public void SetLoadState(IMemento<ItemSaveData> memento)
        {
            base.PopulateLoadState(memento.State);

            ItemType = ItemType.ItemTypes[memento.State.ItemTypeName];
            EnchantmentLevel = memento.State.EnchantmentLevel;
            CurrentRechargeDelay = memento.State.CurrentRechargeDelay;
            IsCharged = memento.State.IsCharged;
            MeleeAttack = (Attack)memento.State.MeleeAttack.Clone();
            DamageShieldPercentage = memento.State.DamageShieldPercentage;
            TotalRechargeDelay = memento.State.TotalRechargeDelay;
            HealPercentOfMax = memento.State.HealPercentOfMax;
            MaxHealthIncrease = memento.State.MaxHealthIncrease;
            GroupsInInventory = memento.State.GroupsInInventory;
            HasBeenDropped = memento.State.HasBeenDropped;
            ItemDiscovery = (ItemDiscovery)memento.State.ItemDiscovery.Clone();
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