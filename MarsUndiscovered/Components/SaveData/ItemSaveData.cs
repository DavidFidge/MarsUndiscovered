namespace MarsUndiscovered.Components.SaveData
{
    public class ItemSaveData : GameObjectSaveData
    {
        public string ItemTypeName { get; set; }
        public int EnchantmentLevel { get; set; }
        public int CurrentRechargeDelay { get; set; }
        public bool IsCharged { get; set; }
        public Attack MeleeAttack { get; set; }
        public int DamageShieldPercentage { get; set; }
        public int TotalRechargeDelay { get; set; }
        public int HealPercentOfMax { get; set; }
        public int MaxHealthIncrease { get; set; }
        public bool GroupsInInventory { get; set; }
        public ItemDiscovery ItemDiscovery { get; set; } = new ItemDiscovery();
    }
}
