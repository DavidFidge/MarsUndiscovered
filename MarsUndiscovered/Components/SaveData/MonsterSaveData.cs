using FrigidRogue.MonoGame.Core.Interfaces.Components;

namespace MarsUndiscovered.Components.SaveData
{
    public class MonsterSaveData : ActorSaveData
    {
        public string BreedName { get; set; }
        public IMemento<MonsterGoalSaveData> MonsterGoalSaveData { get; set; }
    }
}
