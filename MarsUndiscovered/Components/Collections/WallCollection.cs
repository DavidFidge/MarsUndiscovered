using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class WallCollection : GameObjectCollection<Wall, WallSaveData>
    {
        private readonly IGameObjectFactory _gameObjectFactory;

        public WallCollection(IGameObjectFactory gameObjectFactory)
        {
            _gameObjectFactory = gameObjectFactory;
        }

        protected override Wall Create(uint id)
        {
            return _gameObjectFactory.CreateWall(id);
        }
    }
}