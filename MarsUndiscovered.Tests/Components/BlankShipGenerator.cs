using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;

namespace MarsUndiscovered.Tests.Components;

public class BlankShipGenerator : IShipGenerator
{
    public void CreateShip(IGameObjectFactory gameObjectFactory, MarsMap map, ShipCollection shipCollection)
    {
    }
}