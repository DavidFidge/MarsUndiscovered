using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps;

namespace MarsUndiscovered.Tests.Components;

public class BlankShipGenerator : IShipGenerator
{
    public void CreateShip(IGameObjectFactory gameObjectFactory, MarsMap map, ShipCollection shipCollection)
    {
    }
}