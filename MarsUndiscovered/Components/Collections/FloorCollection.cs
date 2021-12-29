using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
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