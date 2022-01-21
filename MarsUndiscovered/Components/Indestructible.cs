namespace MarsUndiscovered.Components
{
    public abstract class Indestructible : MarsGameObject
    {
        public Indestructible(uint id) : base(Constants.IndestructiblesLayer, false, true, () => id)
        {
        }
    }
}