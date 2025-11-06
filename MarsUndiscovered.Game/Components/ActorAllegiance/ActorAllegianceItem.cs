namespace MarsUndiscovered.Game.Components;

public class ActorAllegianceItem
{
    // relationship to other monsters
    public Dictionary<ActorAllegianceItem, ActorAllegianceState> Relationships { get; set; } = new Dictionary<ActorAllegianceItem, ActorAllegianceState>();

    public AllegianceCategory AllegianceCategory { get; set; }
}