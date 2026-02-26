using MarsUndiscovered.Game.Components;

namespace MarsUndiscovered.Game.Components.Dto
{
    public class MonsterStatus : ActorStatus
    {
        public double DistanceFromPlayer { get; set; }
        public string MonsterStateString => MonsterState.ToString().ToSeparateWords();
        public MonsterState MonsterState { get; set; }
        public MonsterState PreviousMonsterState { get; set; }
        public int SearchCooldown { get; set; }
    }
}