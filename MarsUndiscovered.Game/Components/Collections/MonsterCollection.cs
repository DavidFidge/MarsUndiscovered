using FrigidRogue.MonoGame.Core.Interfaces.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components
{
    public class MonsterCollection : GameObjectCollection<Monster, MonsterSaveData>
    {
        public MonsterCollection(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
        }

        protected override void AfterCollectionLoaded(IList<IMemento<MonsterSaveData>> saveData)
        {
            foreach (var monsterSaveData in saveData.Where(s => s.State.LeaderId != null))
            {
                this[monsterSaveData.State.Id].SetLeader(this[monsterSaveData.State.LeaderId.Value]);
            }
        }
    }
}
