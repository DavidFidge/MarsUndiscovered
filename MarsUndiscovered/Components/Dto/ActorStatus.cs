namespace MarsUndiscovered.Components.Dto
{
    public abstract class ActorStatus
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
    }
}