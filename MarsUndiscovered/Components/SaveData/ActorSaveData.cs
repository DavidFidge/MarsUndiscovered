namespace MarsUndiscovered.Components.SaveData
{
    public class ActorSaveData : GameObjectSaveData
    {
        public bool IsDead { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
    }
}
