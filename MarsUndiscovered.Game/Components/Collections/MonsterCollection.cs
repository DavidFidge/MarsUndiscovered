using FrigidRogue.MonoGame.Core.Interfaces.Components;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class MonsterCollection : GameObjectCollection<Monster, MonsterSaveData>
    {
        public MonsterCollection(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
        }

        protected override void AfterCollectionLoaded(IList<IMemento<MonsterSaveData>> saveData, GameWorld gameWorld)
        {
            foreach(var monsterSaveData in saveData)
            {
                if (!monsterSaveData.State.IsDead)
                {
                    var monster = this[monsterSaveData.]
                    Actor leader = GetActorFromId(monsterSaveData.State.LeaderId, gameWorld);
                }
            }

            foreach (var monsterSaveData in saveData.Where(s => s.State.LeaderId != null))
            {
                this[monsterSaveData.State.Id].SetLeader(this[monsterSaveData.State.LeaderId.Value]);
            }
            foreach (var monsterSaveData in saveData.Where(s => s.State.TargetId != null))
            {
                this[monsterSaveData.State.Id].SetLeader(this[monsterSaveData.State.LeaderId.Value]);
            }
            foreach (var monsterSaveData in saveData.Where(s => s.State.TargetOutOfFovId != null))
            {
                this[monsterSaveData.State.Id].SetLeader(this[monsterSaveData.State.LeaderId.Value]);
            }
        }

        private Actor GetActorFromId(uint? id, IGameWorld gameWorld)
        {
            if (id == null)
                return null;

            if (this.TryGetValue(id.Value, out var monster))
                return monster;

            if (id == gameWorld.Player.ID)
                return gameWorld.Player;

            return null;
        }
    }
}
