namespace MarsUndiscovered.Components.SaveData
{
    public abstract class ActorSaveData : GameObjectSaveData
    {
        public bool IsDead { get; set; }
        public int Health { get; set; }
        public int Shield { get; set; }
        public int MaxHealth { get; set; }
        public decimal ResidualRegen { get; set; }
    }
}
