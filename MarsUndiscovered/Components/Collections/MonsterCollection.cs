using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class MonsterCollection : GameObjectCollection<Monster, MonsterSaveData>
    {
        private readonly IGameObjectFactory _gameObjectFactory;

        public MonsterCollection(IGameObjectFactory gameObjectFactory)
        {
            _gameObjectFactory = gameObjectFactory;
        }

        protected override Monster Create(uint id)
        {
            return _gameObjectFactory.CreateMonster(id);
        }
    }
}