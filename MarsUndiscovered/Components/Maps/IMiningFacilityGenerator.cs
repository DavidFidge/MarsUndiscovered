using MarsUndiscovered.Components.Factories;

namespace MarsUndiscovered.Components.Maps
{
    public interface IMiningFacilityGenerator
    {
        void CreateMiningFacility(IGameObjectFactory gameObjectFactory, MarsMap map, MiningFacilityCollection shipCollection);
    }
}
