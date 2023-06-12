using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components
{
    public class MiningFacilityCollection : GameObjectCollection<MiningFacility, MiningFacilitySaveData>
    {
        public MiningFacilityCollection(IGameObjectFactory gameObjectFactory)  : base(gameObjectFactory)
        {
        }
    }
}
