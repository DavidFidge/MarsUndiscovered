using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components
{
    public class MiningFacilityCollection : GameObjectCollection<MiningFacility, MiningFacilitySaveData>
    {
        private readonly IGameObjectFactory _gameObjectFactory;

        public MiningFacilityCollection(IGameObjectFactory gameObjectFactory)
        {
            _gameObjectFactory = gameObjectFactory;
        }

        protected override MiningFacility Create(uint id)
        {
            return _gameObjectFactory.CreateMiningFacility(id);
        }
    }
}
