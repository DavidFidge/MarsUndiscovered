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

        protected override void AfterCollectionLoaded(IGameWorld gameWorld, IList<IMemento<MonsterSaveData>> saveData)
        {
            foreach(var monsterSaveData in saveData)
            {
                if (!monsterSaveData.State.IsDead)
                {
                    var monster = this[monsterSaveData.State.Id];

                    monster.AfterLoad(
                        GetActorFromId(monsterSaveData.State.LeaderId, gameWorld),
                        GetActorFromId(monsterSaveData.State.TargetId, gameWorld),
                        GetActorFromId(monsterSaveData.State.TargetOutOfFovId, gameWorld));
                }
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
