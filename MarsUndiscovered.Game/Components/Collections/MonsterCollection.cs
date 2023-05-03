using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components
{
    public class MonsterCollection : GameObjectCollection<Monster, MonsterSaveData>
    {
        public MonsterCollection(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
        }

        public IEnumerable<Monster> LiveMonsters => Values.Where(m => !m.IsDead).AsEnumerable();
    }
}
