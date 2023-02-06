using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.Maps;

namespace MarsUndiscovered.Tests.Components;

public class BlankMiningFacilityGenerator : IMiningFacilityGenerator
{
    public void CreateMiningFacility(IGameObjectFactory gameObjectFactory, MarsMap map, MiningFacilityCollection shipCollection)
    {
    }
}