using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components
{
    public class FloorCollection : GameObjectCollection<Floor, FloorSaveData>
    {
        private readonly IGameObjectFactory _gameObjectFactory;

        public FloorCollection(IGameObjectFactory gameObjectFactory)
        {
            _gameObjectFactory = gameObjectFactory;
        }

        protected override Floor Create(uint id)
        {
            return _gameObjectFactory.CreateFloor(id);
        }
    }
}