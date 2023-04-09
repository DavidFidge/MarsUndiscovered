using MarsUndiscovered.Game.Components.Factories;

namespace MarsUndiscovered.Game.Components.Maps
{
    public interface IMiningFacilityGenerator
    {
        void CreateMiningFacility(IGameObjectFactory gameObjectFactory, MarsMap map, MiningFacilityCollection shipCollection);
    }
}
