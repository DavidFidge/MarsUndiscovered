using FrigidRogue.MonoGame.Core.Interfaces.Components;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.SaveData
{
    public class MonsterSaveData : ActorSaveData
    {
        public string BreedName { get; set; }
        public IList<IMemento<SeenTileSaveData>> SeenTiles { get; set; }
        public List<Point> WanderPath { get; set; }
        public bool UseGoalMapWander { get; set; }
        public uint? LeaderId { get; set; }
        public uint? TargetId { get; set; }
        public uint? TargetOutOfFovId { get; set; }
        public MonsterState MonsterState { get; set; }
    }
}
