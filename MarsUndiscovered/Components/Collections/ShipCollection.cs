
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class ShipCollection : GameObjectCollection<Ship, ShipSaveData>
    {
        private readonly IGameObjectFactory _gameObjectFactory;

        public ShipCollection(IGameObjectFactory gameObjectFactory)
        {
            _gameObjectFactory = gameObjectFactory;
        }

        protected override Ship Create(uint id)
        {
            return _gameObjectFactory.CreateShip(id);
        }
    }
}