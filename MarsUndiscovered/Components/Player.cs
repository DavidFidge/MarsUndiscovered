namespace MarsUndiscovered.Components
{
    public class Player : MarsGameObject
    {
        public Player() : base(1, true)
        {
        }

        public Player(uint id) : base(1, true, idGenerator: () => id)
        {
        }
    }
}