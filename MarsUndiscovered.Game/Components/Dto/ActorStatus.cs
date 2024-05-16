using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.Dto
{
    public abstract class ActorStatus
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Shield { get; set; }
        public Point Position { get; set; }
        public bool CanBeConcussed { get; set; }
        public bool IsConcussed { get; set; }
    }
}