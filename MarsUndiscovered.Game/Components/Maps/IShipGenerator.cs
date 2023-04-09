using MarsUndiscovered.Game.Components.Factories;

namespace MarsUndiscovered.Game.Components.Maps
{
    public interface IShipGenerator
    {
        void CreateShip(IGameObjectFactory gameObjectFactory, MarsMap map, ShipCollection shipCollection);
    }
}
