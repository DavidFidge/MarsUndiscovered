namespace MarsUndiscovered.Components.SaveData
{
    public class ItemSaveData : GameObjectSaveData
    {
        public string ItemTypeName { get; set; }
        public int PowerLevel { get; set; }
        public int CurrentRechargeDelay { get; set; }
        public bool IsCharged { get; set; }
        public Attack MeleeAttack { get; set; }
        public int DamageShieldPercentage { get; set; }
        public int TotalRechargeDelay { get; set; }
    }
}
