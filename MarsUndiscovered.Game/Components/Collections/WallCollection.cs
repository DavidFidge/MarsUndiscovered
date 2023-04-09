using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components
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