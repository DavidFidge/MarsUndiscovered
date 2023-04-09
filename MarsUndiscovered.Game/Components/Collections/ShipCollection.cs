using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components
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
