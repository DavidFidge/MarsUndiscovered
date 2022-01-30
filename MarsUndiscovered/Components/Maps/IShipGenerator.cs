
using MarsUndiscovered.Components.Factories;

namespace MarsUndiscovered.Components.Maps
{
    public interface IShipGenerator
    {
        void CreateShip(IGameObjectFactory gameObjectFactory, MarsMap map, ShipCollection shipCollection);
    }
}