namespace MarsUndiscovered.Game.Components.Factories;

public class CommandCollectionSaveData
{
    public uint NextId { get; set; }
    public List<uint> ReplayCommandIds { get; set; } = new();
}