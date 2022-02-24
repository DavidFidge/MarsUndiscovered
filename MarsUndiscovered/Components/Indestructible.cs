using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public abstract class Indestructible : MarsGameObject
    {
        public Indestructible(IGameWorld gameWorld, uint id) : base(gameWorld, Constants.IndestructiblesLayer, false, true, () => id)
        {
        }
    }
}