namespace MarsUndiscovered.Components.SaveData;

public class MorgueSaveData
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Dictionary<string, int> EnemiesDefeated { get; set; } = new();
}