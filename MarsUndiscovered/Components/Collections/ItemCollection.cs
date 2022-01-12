using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class ItemCollection : GameObjectCollection<Wall, WallSaveData>
    {
        private readonly IGameObjectFactory _gameObjectFactory;

        public ItemCollection(IGameObjectFactory gameObjectFactory)
        {
            _gameObjectFactory = gameObjectFactory;
        }

        protected override Item Create(uint id)
        {
            return _gameObjectFactory.CreateWall(id);
        }
    }
}