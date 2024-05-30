namespace MarsUndiscovered.Game.Components.Dto
{
    public class MonsterStatus : ActorStatus
    {
        public double DistanceFromPlayer { get; set; }
        public string Behaviour { get; set; }
        public int SearchCooldown { get; set; }
    }
}