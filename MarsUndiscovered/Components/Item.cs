using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class Item : MarsGameObject, IMementoState<ItemSaveData>
    {
        public ItemType ItemType { get; set; }
        public int EnchantmentLevel { get; set; }
        public int CurrentRechargeDelay { get; set; }
        public int TotalRechargeDelay { get; set; }

        public bool IsEquipped { get; set; }
        public bool IsCharged { get; set; }
        public Attack MeleeAttack { get; set; }
        public Attack LineAttack { get; set; }
        public int DamageShieldPercentage { get; set; }
        public int HealPercentOfMax { get; set; }
        public int MaxHealthIncrease { get; set; }
        public bool GroupsInInventory { get; set; }
        public bool HasBeenDropped { get; set; }
        public ItemDiscovery ItemDiscovery { get; set; } = new ItemDiscovery();

        public Item(IGameWorld gameWorld, uint id) : base(gameWorld, Constants.ItemLayer, true, true, () => id)
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
            var memento = new Memento<ItemSaveData>(new ItemSaveData());

            base.PopulateSaveState(memento.State);

            memento.State.ItemTypeName = ItemType.Name;
            memento.State.EnchantmentLevel = EnchantmentLevel;
            memento.State.CurrentRechargeDelay = CurrentRechargeDelay;
            memento.State.IsCharged = IsCharged;
            memento.State.IsEquipped = IsEquipped;

            if (MeleeAttack != null)
                memento.State.MeleeAttack = (Attack)MeleeAttack.Clone();

            if (LineAttack != null)
                memento.State.LineAttack = (Attack)LineAttack.Clone();
            
            memento.State.DamageShieldPercentage = DamageShieldPercentage;
            memento.State.TotalRechargeDelay = TotalRechargeDelay;
            memento.State.HealPercentOfMax = HealPercentOfMax;
            memento.State.MaxHealthIncrease = MaxHealthIncrease;
            memento.State.GroupsInInventory = GroupsInInventory;
            memento.State.HasBeenDropped = HasBeenDropped;

            if (memento.State.ItemDiscovery != null)
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
            IsEquipped = memento.State.IsEquipped;

            if (memento.State.MeleeAttack != null)
                MeleeAttack = (Attack)memento.State.MeleeAttack.Clone();

            if (memento.State.LineAttack != null)
                LineAttack = (Attack)memento.State.LineAttack.Clone();

            DamageShieldPercentage = memento.State.DamageShieldPercentage;
            TotalRechargeDelay = memento.State.TotalRechargeDelay;
            HealPercentOfMax = memento.State.HealPercentOfMax;
            MaxHealthIncrease = memento.State.MaxHealthIncrease;
            GroupsInInventory = memento.State.GroupsInInventory;
            HasBeenDropped = memento.State.HasBeenDropped;

            if (ItemDiscovery != null)
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
        
        public string GetDescriptionWithoutStatus(ItemTypeDiscovery itemTypeDiscovery)
        {
            return ItemType.GetDescription(this, ItemDiscovery, itemTypeDiscovery, 1, true, false);
        }
        
        public string GetDescriptionWithoutPrefix(ItemTypeDiscovery itemTypeDiscovery)
        {
            return ItemType.GetDescription(this, ItemDiscovery, itemTypeDiscovery, 0, false);
        }
        
        public string GetDiscoveredDescription(int quantity)
        {
            return ItemType.GetDescription(this, ItemDiscovery.ItemDiscoveryDiscovered, ItemTypeDiscovery.ItemTypeDiscoveryDiscovered, quantity);
        }

        public string GetLongDescription(ItemTypeDiscovery itemTypeDiscovery)
        {
            return ItemType.GetLongDescription(this, itemTypeDiscovery);
        }
        
        public string GetEnchantmentLevelText()
        {
            return
                $"With the current enchantment level of {(EnchantmentLevel >= 0 ? "{{L_BLUE}}+" : "{{L_RED}}-")}{EnchantmentLevel}{{{{DEFAULT}}}}";
        }
        
        public string GetRechargeText()
        {
            if (CurrentRechargeDelay == 0)
                return "ready";

            return $"{((TotalRechargeDelay - CurrentRechargeDelay) * 100) / TotalRechargeDelay}% charged";
        }

        public string GetRechargeLongDescription()
        {
            var afterUseText = $"After use, it will take {TotalRechargeDelay} turns to recharge.";
            
            if (CurrentRechargeDelay == 0)
                return $"The item is ready to use. {afterUseText}";

            return $"The item is recharging and will be ready to use in {CurrentRechargeDelay} turns. {afterUseText}";
        }

        public void Recharge()
        {
            if (CurrentRechargeDelay > 0)
                CurrentRechargeDelay--;
        }
        
        public void FullRecharge()
        {
            CurrentRechargeDelay = 0;
        }

        public void ResetRechargeDelay()
        {
            CurrentRechargeDelay = TotalRechargeDelay;
        }
    }
}
