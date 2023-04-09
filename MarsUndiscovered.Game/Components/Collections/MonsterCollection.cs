using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components
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

        public IEnumerable<Monster> LiveMonsters => Values.Where(m => !m.IsDead).AsEnumerable();
    }
}
