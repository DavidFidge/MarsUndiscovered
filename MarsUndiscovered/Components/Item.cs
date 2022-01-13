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

        public Item(uint id) : base(2, true, true, () => id)
        {
        }

        public Item WithItemType(ItemType itemType)
        {
            ItemType = itemType;
            ItemType.ApplyEnchantmentLevel(this);
            ItemType.ApplyProperties(this);

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
    }
}