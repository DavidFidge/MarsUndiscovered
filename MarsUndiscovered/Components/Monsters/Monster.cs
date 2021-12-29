using FrigidRogue.MonoGame.Core.Interfaces.Components;

namespace MarsUndiscovered.Components
{
    public class Monster : Actor
    {
        private Breed Breed { get; set; }

        public int Health { get; set; }

        public Monster() : base(1, true)
        {
        }

        public Monster(uint id) : base(1, true, idGenerator: () => id)
        {
        }
    }
}