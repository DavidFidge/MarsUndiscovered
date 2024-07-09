namespace MarsUndiscovered.Game.Components.Dto
{
    public class PlayerStatus : ActorStatus
    {
        public bool IsDead { get; set; }
        public bool IsVictorious { get; set; }
        public string AmbientText { get; set; }
    }
}